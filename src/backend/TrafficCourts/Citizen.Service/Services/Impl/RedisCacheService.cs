using StackExchange.Redis;
using System.Text.Json;

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

        public async Task<T> GetRecordAsync<T>(string key)
        {
            try
            {
                var jsonData = await _redisDbAsync.StringGetAsync(key);

                if (jsonData.IsNull)
                {
                    return default(T);
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
    }
}
