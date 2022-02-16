using Minio;
using Minio.Exceptions;
using NodaTime;
using TrafficCourts.Common;

namespace TrafficCourts.Citizen.Service.Services
{
    /// <summary>
    /// Saves the file to S3 compatible 
    /// </summary>
    public class MinioFilePersistenceService : FilePersistenceService
    {
        private readonly MinioClient _client;
        private readonly MinioConfiguration _configuration;
        private readonly IClock _clock;

        public MinioFilePersistenceService(
            MinioClient client,
            MinioConfiguration configuration,
            IClock clock,
            ILogger<MinioFilePersistenceService> logger)
            : base(logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public override Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override async Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken)
        {
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
                bool found = await _client.BucketExistsAsync(_configuration.BucketName, cancellationToken);
                if (!found)
                {
                    await _client.MakeBucketAsync(_configuration.BucketName, _configuration.Location, cancellationToken);
                }

                // Upload a file to bucket.
                await _client.PutObjectAsync(_configuration.BucketName, objectName, data, data.Length, mimeType.Name, cancellationToken: cancellationToken);
                return filename;
            }
            catch (MinioException exception)
            {
                _logger.LogError(exception, "Failed up upload file");
                throw new MinioFilePersistenceException("Failed up upload file", exception);
            }
        }
    }



}
