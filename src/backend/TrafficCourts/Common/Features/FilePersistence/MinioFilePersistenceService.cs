using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using NodaTime;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using TrafficCourts.Common.Configuration;
using Microsoft.Extensions.Hosting;

namespace TrafficCourts.Common.Features.FilePersistence;

/// <summary>
/// Saves the file to S3 compatible 
/// </summary>
public class MinioFilePersistenceService : FilePersistenceService
{
    private IObjectOperations _objectOperations;
    private readonly IMemoryStreamManager _memoryStreamManager;
    private readonly IClock _clock;
    private readonly string _bucketName;
    private const string CreatedAtDateTimeFormat = "yyyy-MM-ddTHH:mm";
    private const string CreatedAtHeader = "createdAt";
    private const string TypeHeader = "type";
    private bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;


    public MinioFilePersistenceService(
        IObjectOperations objectOperations,
        IOptions<ObjectBucketConfiguration> objectBucketConfiguration,
        IMemoryStreamManager memoryStreamManager,
        IClock clock,
        ILogger<MinioFilePersistenceService> logger)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(objectOperations);
        _objectOperations = objectOperations;
        _memoryStreamManager = memoryStreamManager ?? throw new ArgumentNullException(nameof(memoryStreamManager));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _bucketName = objectBucketConfiguration.Value.BucketName;
    }

    public override async Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> {
            ["FileName"] = filename,
            ["BucketName"] = _bucketName,
        });

        try
        {
            MemoryStream objectStream = _memoryStreamManager.GetStream();

            GetObjectArgs args = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(filename)
                .WithCallbackStream((stream) => { stream.CopyTo(objectStream); });

            ObjectStat? status = await _objectOperations.GetObjectAsync(args, cancellationToken);
            return objectStream;
        }
        catch (BucketNotFoundException exception)
        {
            _logger.LogInformation(exception, "Bucket not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (ObjectNotFoundException exception)
        {
            _logger.LogInformation(exception, "Object not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            _logger.LogInformation(exception, "Directory not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Error fetching file from object storage");
            throw new MinioFilePersistenceException("Error getting file", exception); // TODO: add exception parameter that handles Exception data type
        }
    }

    public override async Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length == 0) throw new ArgumentException("No data to save", nameof(data));

        var mimeType = data.GetMimeType();
        if (mimeType is null)
        {
            _logger.LogInformation("Could not determine mime type for file, file cannot be saved");
            return string.Empty;
        }

        var filename = GetFileName(mimeType);
        var now = _clock.GetCurrentPacificTime();
        string objectName = $"{now:yyyy-MM-dd}/{filename}";

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["FileName"] = filename,
            ["BucketName"] = _bucketName,
        });

        try
        {
            // bucket must exist prior to calling this otherwise BucketNotFoundException will be thrown
            // in the shared services object store, we are not giving permissions to list or create buckets

            // Upload a file to bucket.
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithContentType(mimeType.MimeType)
                    .WithObjectSize(data.Length)
                    .WithStreamData(data);

            await _objectOperations.PutObjectAsync(putObjectArgs, cancellationToken);
            return objectName;
        }
        catch (BucketNotFoundException exception)
        {
            _logger.LogWarning(exception, "Object Store bucket not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (MinioException exception)
        {
            _logger.LogError(exception, "Failed to upload file to object storage");
            throw new MinioFilePersistenceException("Failed to upload file to object storage", exception);
        }
    }

    public override async Task<string> SaveJsonFileAsync<T>(T data, string filename, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (string.IsNullOrEmpty(filename)) throw new ArgumentException("No filename provided to save", nameof(filename));

        // The type of the data that can be dynamically determined and used to deserialize
        Type dataType = typeof(T);
        string? objectType = dataType.FullName;
        _logger.LogDebug("The object type of the saved data is {ObjectType}", objectType);

        string createdAt = _clock.GetCurrentInstant().ToDateTimeUtc().ToString(CreatedAtDateTimeFormat);

        // Convert the provided data into UTF-8 encoded JSON and write it to stream to save as a file
        using var dataJsonStream = _memoryStreamManager.GetStream();
        JsonSerializer.Serialize(dataJsonStream, data);
        // Reset file position
        dataJsonStream.Position = 0L;

        Debug.Assert(dataJsonStream.Length != 0);

        // The metadata (object type and created timestamp) of the json object that will be saved in object store
        Dictionary<string, string> headers = new();
        headers.Add(CreatedAtHeader, createdAt);
        if (objectType != null)
        {
            headers.Add(TypeHeader, objectType);
        }

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["FileName"] = filename,
            ["BucketName"] = _bucketName,
        });

        try
        {
            // bucket must exist prior to calling this otherwise BucketNotFoundException will be thrown
            // in the shared services object store, we are not giving permissions to list or create buckets

            // Upload a file to bucket.
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(filename)
                    .WithContentType("application/json")
                    .WithObjectSize(dataJsonStream.Length)
                    .WithStreamData(dataJsonStream)
                    .WithHeaders(headers);

            await _objectOperations.PutObjectAsync(putObjectArgs, cancellationToken);
            
            return filename;
        }
        catch (BucketNotFoundException exception)
        {
            _logger.LogWarning(exception, "Object Store bucket not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (MinioException exception)
        {
            _logger.LogError(exception, "Failed to upload file to object storage");
            throw new MinioFilePersistenceException("Failed to upload file to object storage", exception);
        }
    }

    public override async Task<T?> GetJsonDataAsync<T>(string filename, CancellationToken cancellationToken) where T : class
    {
        ArgumentNullException.ThrowIfNull(filename);

        Type targetType = typeof(T);

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["FileName"] = filename,
            ["BucketName"] = _bucketName,
        });

        try
        {
            MemoryStream objectStream = _memoryStreamManager.GetStream();

            GetObjectArgs args = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(filename)
                .WithCallbackStream((stream) => { stream.CopyTo(objectStream); });

            ObjectStat? status = await _objectOperations.GetObjectAsync(args, cancellationToken);

            if (status == null)
            {
                _logger.LogInformation("Could not fetch object from object storage with objectStat and headers");
                return null;
            }
            Dictionary<string, string> headers = status.MetaData;
            if (headers.TryGetValue(TypeHeader, out string? objectType))
            {
                if (objectType != targetType.FullName)
                {
                    _logger.LogWarning("Target object type to return: {targetType} does not match the type of json object fetched from object storage: {objectType}", targetType.FullName, objectType);
                    return null;
                }
            }
            // Reset file position
            objectStream.Position = 0L;

            return JsonSerializer.Deserialize<T>(objectStream);
        }
        catch (BucketNotFoundException exception)
        {
            _logger.LogInformation(exception, "Bucket not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (ObjectNotFoundException exception)
        {
            _logger.LogInformation(exception, "Object not found");
            if (!isDevelopment) throw new FileNotFoundException("File not found", filename, exception); // allow this in dev and docker-compose
            else return null;
        }
        catch (DirectoryNotFoundException exception)
        {
            _logger.LogInformation(exception, "Directory not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Error fetching file from object storage");
            throw new MinioFilePersistenceException("Error getting file", exception);
        }
    }
}
