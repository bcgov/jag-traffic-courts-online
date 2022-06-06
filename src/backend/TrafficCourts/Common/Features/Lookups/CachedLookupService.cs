using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace TrafficCourts.Common.Features.Lookups
{
    /// <summary>
    /// A cached look up service. Fetches data from Redis and optional caches locally to avoid 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CachedLookupService<T> : ICachedLookupService<T>
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly IConnectionMultiplexer _redis;
        protected readonly string _key;
        protected readonly IMemoryCache _cache;
        protected readonly TimeSpan _cacheTime;
        protected readonly ILogger<CachedLookupService<T>> _logger;

        /// <summary>
        /// A cached look up service. Fetches data from Redis and optional caches locally to avoid 
        /// </summary>
        /// <param name="key">The key to cache the item with.</param>
        /// <param name="redis">The redis connection.</param>
        /// <param name="cache">The optional cache if local caching is required. Required if <paramref name="cacheTime"/> is greater than <see cref="TimeSpan.Zero"/></param>
        /// <param name="cacheTime">The amount of time to cache locally.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected CachedLookupService(string key, IConnectionMultiplexer redis, IMemoryCache cache, TimeSpan cacheTime, ILogger<CachedLookupService<T>> logger)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));

            _cacheTime = cacheTime;
            if (cacheTime > TimeSpan.Zero && cache is null)
            {
                throw new ArgumentNullException(nameof(cache), "cache parameter is required if cacheTime is greater than zero");
            }

            _cache = cache;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IList<T>> GetListAsync()
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["Key"] = _key });

            T[]? values;

            if (_cacheTime > TimeSpan.Zero)
            {
                values = _cache.Get<T[]>(_key);
                if (values is not null)
                {
                    _logger.LogDebug("Found locally cached {Count} entries of {Type}", values.Length, typeof(T).Name);
                    return values;
                }
            }

            values = await GetListFromRedisAsync();

            if (values is not null && values.Length > 0 && _cacheTime > TimeSpan.Zero)
            {
                _logger.LogDebug("Locally caching {Count} entries of {Type} for {Time}", values.Length, typeof(T).Name, _cacheTime);
                _cache.Set(_key, values, _cacheTime);
            }

            if (values is null)
            {
                _logger.LogInformation("Could not locate data for {Type}, returning empty collection", typeof(T).Name);
                return Array.Empty<T>();
            }

            // TODO: should I return a new List<T>? Depends if the caller wants sort the returned value, for now dont.
            return values;
        }

        private async Task<T[]> GetListFromRedisAsync()
        {
            IDatabase database = _redis.GetDatabase();
            RedisValue value = await database.StringGetAsync(_key);

            if (value.IsNullOrEmpty)
            {
                _logger.LogInformation("Could not get value from redis key, returning empty collection");
                return Array.Empty<T>();
            }

            try
            {
                string json = value; // implicit operator allows RedisValue to string conversion
                T[]? values = JsonSerializer.Deserialize<T[]>(json, _jsonSerializerOptions);
                if (values is null)
                {
                    _logger.LogError("Deserializing redis value returned null, returning empty collection", typeof(T).Name);
                    return Array.Empty<T>();
                }

                return values;
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Error deserializing redis value into an array of {Type}, returning empty collection", typeof(T).Name);
                return Array.Empty<T>();
            }
        }
    }
}
