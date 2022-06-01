using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using NodaTime;

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
            _logger.LogInformation("Bucket not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (ObjectNotFoundException exception)
        {
            _logger.LogInformation("Object not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            _logger.LogInformation("Directory not found");
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
            _logger.LogWarning("Object Store bucket not found");
            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (MinioException exception)
        {
            _logger.LogError(exception, "Failed to upload file to object storage");
            throw new MinioFilePersistenceException("Failed to upload file to object storage", exception);
        }
    }
}
