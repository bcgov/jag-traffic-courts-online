using StackExchange.Redis;
using System.Text.Json;
using System.Buffers;
using System.Diagnostics;
using Winista.Mime;

namespace TrafficCourts.Citizen.Service.Services.Impl
{
    /// <summary>
    /// Implentation of the IRedisCacheService that is used for saving and retrieving json serialized generic data to/from Redis.
    /// </summary>
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabaseAsync _redisDbAsync;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _redisDbAsync = _redis.GetDatabase(); // Obtain the connection to database
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T?> GetRecordAsync<T>(string key)
        {
            try
            {
                var jsonData = await _redisDbAsync.StringGetAsync(key);

                if (jsonData.IsNull)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not retrieve data record from Redis cache.");
                throw;
            }
        }

        public async Task SetRecordAsync<T>(string key, T data, TimeSpan? expireTime = null)
        {
           var jsonData = JsonSerializer.Serialize(data);
           await _redisDbAsync.StringSetAsync(key, jsonData, expireTime ?? TimeSpan.FromDays(1));
        }

        public async Task<MemoryStream> GetFileRecordAsync<T>(string key)
        {
            try
            {
                var fileData = await _redisDbAsync.StringGetAsync(key);

                if (fileData.IsNull)
                {
                    return default;
                }

                var stream = new MemoryStream(fileData);
                return stream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not retrieve data record from Redis cache.");
                throw;
            }
        }

        public async Task<string> SetFileRecordAsync(MemoryStream data, TimeSpan? expireTime = null)
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

            string objectName = $"{DateTime.Now:yyyy-MM-dd}/{filename}";
            _logger.LogInformation($"Saving file: {objectName} to database.");

            var buffer = data.ToArray();
            await _redisDbAsync.StringSetAsync(objectName, buffer, expireTime ?? TimeSpan.FromDays(1));

            return objectName;
        }

        protected async Task<MimeType> GetMimeTypeAsync(Stream data)
        {
            var pool = ArrayPool<byte>.Shared;
            // get a buffer to read the first 1K of the file stream
            // any of our images should be at least 1KB be.
            var buffer = pool.Rent(1024);

            try
            {
                int count = await data.ReadAsync(buffer, 0, buffer.Length);
                var mimeTypes = new MimeTypes();
                MimeType mimeType = mimeTypes.GetMimeType(buffer);
                return mimeType;
            }
            finally
            {
                data.Seek(0L, SeekOrigin.Begin); // reset the memory stream to the beginning
                pool.Return(buffer);
            }
        }

        protected string GetFileName(MimeType mimeType)
        {
            string id = Guid.NewGuid().ToString("n");

            // even with an empty buffer, MimeTypes.GetMimeType always seems to return a mime type
            if (mimeType.Extensions is null || mimeType.Extensions.Length == 0)
            {
                _logger.LogDebug("No mime type or extension available");
                return id;
            }

            string extension = mimeType.Extensions[0];
            Debug.Assert(extension is not null);
            return $"{id}.{extension}";
        }
    }
}
