using Microsoft.Extensions.Logging;
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
    private readonly MinioConfiguration _configuration;
    private readonly IClock _clock;

    private IBucketOperations _bucketOperations;
    private IObjectOperations _objectOperations;

    public MinioFilePersistenceService(
        MinioClient client,
        MinioConfiguration configuration,
        IClock clock,
        ILogger<MinioFilePersistenceService> logger)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        _bucketOperations = client;
        _objectOperations = client;

        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public override async Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken)
    {
        try
        {
            MemoryStream objectStream = new MemoryStream();

            GetObjectArgs args = new GetObjectArgs()
                .WithBucket(_configuration.BucketName)
                .WithFile(filename)
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
            throw new MinioFilePersistenceException("Error getting file", null); // TODO: add exception parameter that handles Exception data type
        }
    }

    public override async Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length == 0) throw new ArgumentException("No data to save", nameof(data))

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
            // Make a bucket on the server, if not already present.
            BucketExistsArgs bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(_configuration.BucketName);

            bool found = await _bucketOperations.BucketExistsAsync(bucketExistsArgs, cancellationToken);
            if (!found)
            {
                MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_configuration.BucketName)
                    .WithLocation(_configuration.Location);

                await _bucketOperations.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }

            // Upload a file to bucket.
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(_configuration.BucketName)
                    .WithFileName(objectName)
                    .WithContentType(mimeType.Name)
                    .WithObjectSize(data.Length)
                    .WithStreamData(data);

            await _objectOperations.PutObjectAsync(putObjectArgs, cancellationToken);
            return objectName;
        }
        catch (MinioException exception)
        {
            _logger.LogError(exception, "Failed up upload file");
            throw new MinioFilePersistenceException("Failed up upload file", exception);
        }
    }

    /// <summary>
    /// For unit testing, allows overriding the <see cref="IBucketOperations"/> implemetation.
    /// </summary>
    public void SetBucketOperations(IBucketOperations bucketOperations) => _bucketOperations = bucketOperations;
    /// <summary>
    /// For unit testing, allows overriding the <see cref="IObjectOperations"/> implemetation.
    /// </summary>
    public void SetObjectOperations(IObjectOperations objectOperations) => _objectOperations = objectOperations;
}



