using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TrafficCourts.Core.Http;
using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Http;

public partial class TokenCache : ITokenCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TokenCache> _logger;
    private readonly Func<DateTimeOffset> _utcNow = () => DateTimeOffset.UtcNow;

    public TokenCache(IMemoryCache memoryCache, ILogger<TokenCache> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Token? GetToken(OidcConfidentialClientConfiguration key)
    {
        ArgumentNullException.ThrowIfNull(key);

        // prefix each cache with the key's 
        string cacheKey = key.GetCacheKey();
        LogLookingUpToken(cacheKey);

        if (_memoryCache.TryGetValue(cacheKey, out Token? token))
        {
            LogTokenFound();
            return token;
        }

        LogTokenNotFound();
        return null;
    }

    public void SaveToken(OidcConfidentialClientConfiguration key, Token token, DateTimeOffset tokenExpiresAtUtc)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(token);

        DateTimeOffset now = _utcNow();

        if (tokenExpiresAtUtc <= now)
        {
            LogTokenExpired(now, tokenExpiresAtUtc);
            return; // token is already expired
        }

        string cacheKey = key.GetCacheKey();

        LogCachingToken(cacheKey, tokenExpiresAtUtc);

        _memoryCache.Set(cacheKey, token, tokenExpiresAtUtc);
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Getting token using {CacheKey}")]
    public partial void LogLookingUpToken(string cacheKey);

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Cached token found")]
    public partial void LogTokenFound();

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Cached token not found")]
    public partial void LogTokenNotFound();

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Token is already expired, not adding to cache. The current time is {Now} and the token expired at {ExpiresAtUtc}")]
    public partial void LogTokenExpired(DateTimeOffset now, DateTimeOffset expiresAtUtc);

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Caching token using cache key {CacheKey} until {ExpiresAtUtc}")]
    public partial void LogCachingToken(string cacheKey, DateTimeOffset expiresAtUtc);
}
