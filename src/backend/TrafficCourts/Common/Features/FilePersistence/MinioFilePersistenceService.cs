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
            throw new MinioFilePersistenceException("Error getting file", null!); // TODO: add exception parameter that handles Exception data type
        }
    }

    public override async Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length == 0) throw new ArgumentException("No data to save", nameof(data));

        var mimeType = await GetMimeTypeAsync(data);
        if (mimeType is null)
        {
            _logger.LogInformation("Could not determine mime type for file, file cannot be saved");
            return string.Empty;
        }

        var filename = GetFileName(mimeType);
        if (filename == string.Empty)
        {
            return string.Empty;
        }

        var now = _clock.GetCurrentPacificTime();
        string objectName = $"{now:yyyy-MM-dd}/{filename}";

        try
        {
            // bucket must exist prior to calling this otherwise BucketNotFoundException will be thrown
            // in the shared services object store, we are not giving permissions to list or create buckets

            // Upload a file to bucket.
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithContentType(mimeType.Name)
                    .WithObjectSize(data.Length)
                    .WithStreamData(data);

            await _objectOperations.PutObjectAsync(putObjectArgs, cancellationToken);
            return objectName;
        }
        catch (BucketNotFoundException exception)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "BucketName", _bucketName } });
            _logger.LogWarning("Object Store bucket not found");

            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (MinioException exception)
        {
            _logger.LogError(exception, "Failed to upload file to object storage");
            throw new MinioFilePersistenceException("Failed to upload file to object storage", exception);
        }
    }
    public override async Task DeleteFileAsync(string filename, CancellationToken cancellationToken)
    {
        try
        {
            RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(filename);
            await _objectOperations.RemoveObjectAsync(removeObjectArgs, cancellationToken);
        }
        catch (BucketNotFoundException exception)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "BucketName", _bucketName } });
            _logger.LogWarning("Object Store bucket not found");

            throw new FileNotFoundException("File not found", filename, exception);
        }
        catch (MinioException exception)
        {
            _logger.LogError(exception, "Failed to remove file");
            throw new MinioFilePersistenceException("Failed to remove file", exception);
        }
    }
}
