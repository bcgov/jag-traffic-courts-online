using StackExchange.Redis;
using System.Text.Json;
using System.Buffers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace TrafficCourts.Citizen.Service.Services.Impl
{
    /// <summary>
    /// Implentation of the IRedisCacheService that is used for saving and retrieving json serialized generic data to/from Redis.
    /// </summary>
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabaseAsync _redisDbAsync;
        private readonly IMemoryStreamManager _memoryStreamManager;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, IMemoryStreamManager memoryStreamManager, ILogger<RedisCacheService> logger)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _redisDbAsync = _redis.GetDatabase(); // Obtain the connection to database
            _memoryStreamManager = memoryStreamManager;
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
                _logger.LogError(ex, "Could not retrieve data record from Redis cache");
                throw;
            }
        }

        public async Task SetRecordAsync<T>(string key, T data, TimeSpan? expireTime = null)
        {
           var jsonData = JsonSerializer.Serialize(data);
           await _redisDbAsync.StringSetAsync(key, jsonData, expireTime ?? TimeSpan.FromDays(1));
        }

        public async Task<MemoryStream?> GetFileRecordAsync(string key)
        {
            var fileData = await _redisDbAsync.StringGetAsync(key);

            if (fileData.IsNull)
            {
                return default;
            }

            MemoryStream stream = _memoryStreamManager.GetStream();

            int size = Convert.ToInt32(fileData.Length()); // Don't think it should ever reach max of int, which is ~2 gigs
            await stream.WriteAsync(fileData, 0, size);

            stream.Position = 0;
            return stream;
        }

        public async Task<string> SetFileRecordAsync(string key, MemoryStream data, TimeSpan? expireTime = null)
        {
            if (key is null || key == string.Empty)
            {
                _logger.LogError("No key was provided for the record, file cannot be saved");
                return string.Empty;
            }

            var filename = $"{key}-File";
            _logger.LogInformation("Saving file: {FileName} to redis cache.", filename);

            var buffer = data.ToArray();
            await _redisDbAsync.StringSetAsync(filename, buffer, expireTime ?? TimeSpan.FromDays(1));

            return filename;
        }

        public async Task<bool> DeleteRecordAsync(string key)
        {
            if (key is null || key == string.Empty)
            {
                _logger.LogError("No key was provided, record cannot be deleted");
                return false;
            }

            var deleteSuccessful = await _redisDbAsync.KeyDeleteAsync(key);
            return deleteSuccessful;
        }

    }
}
