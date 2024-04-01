using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace TrafficCourts.Caching;

public static class FusionCacheExtensions
{
    /// <summary>
    /// Adds a named fusion cache instance with a cache key prefix.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="cacheName"></param>
    /// <param name="cacheKeyPrefix">The cache key prefix to use.</param>
    /// <returns></returns>
    public static IFusionCacheBuilder AddFusionCache(IServiceCollection services, string cacheName, string? cacheKeyPrefix = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheName);

        var builder = services.AddFusionCache(cacheName)
            .WithSerializer(new FusionCacheSystemTextJsonSerializer());

        if (cacheKeyPrefix is not null)
        {
            builder.WithCacheKeyPrefix(cacheKeyPrefix);
        }

        return builder;
    }

    /// <summary>
    /// Adds standard distributed
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="redisConnectionString">The Redis connection string</param>
    /// <returns></returns>
    public static IFusionCacheBuilder WithCommonDistributedCacheOptions(
        this IFusionCacheBuilder builder,
        string redisConnectionString,
        Action<FusionCacheOptions>? configureOptions = null,
        Action<FusionCacheEntryOptions>? configureCacheEntryOptions = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(redisConnectionString);

        // values below come from the step by step guide
        // https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/StepByStep.md#8-backplane-more
        // note: the cache duration is based on the result set

        // build our cache options
        FusionCacheOptions options = new()
        {
            DistributedCacheCircuitBreakerDuration = TimeSpan.FromSeconds(2)
        };

        configureOptions?.Invoke(options);

        // build our cache entry options
        FusionCacheEntryOptions entryOptions = new()
        {
            AllowBackgroundDistributedCacheOperations = true
        };

        configureCacheEntryOptions?.Invoke(entryOptions);

        builder
            .WithOptions(options)
            .WithDefaultEntryOptions(entryOptions)
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(new RedisCache(new RedisCacheOptions() { Configuration = redisConnectionString }))
            .WithBackplane(new RedisBackplane(new RedisBackplaneOptions() { Configuration = redisConnectionString }))
            ;

        return builder;
    }
}