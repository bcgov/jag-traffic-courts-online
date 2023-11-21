using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TrafficCourts.Core.Http;
using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Http;

public class TokenCache : ITokenCache
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
        if (key is null) throw new ArgumentNullException(nameof(key));

        // prefix each cache with the key's 
        string cacheKey = GetCacheKey(key);
        _logger.LogTrace("Getting token using {CacheKey}", cacheKey);

        if (_memoryCache.TryGetValue(cacheKey, out Token? token))
        {
            _logger.LogTrace("Cached token found");
            return token;
        }

        _logger.LogTrace("Cached token not found");
        return null;
    }

    public void SaveToken(OidcConfidentialClientConfiguration key, Token token, DateTimeOffset tokenExpiresAtUtc)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (token is null) throw new ArgumentNullException(nameof(token));

        DateTimeOffset now = _utcNow();

        if (tokenExpiresAtUtc <= now)
        {
            _logger.LogDebug("Token is already expired, not adding to cache. The current time is {Now} and the token expired at {ExpiresAtUtc}",
                now,
                tokenExpiresAtUtc);
            return; // token is already expired
        }

        string cacheKey = GetCacheKey(key);
        _logger.LogTrace("Caching token using cache key {CacheKey} until {ExpiresAtUtc}", cacheKey, tokenExpiresAtUtc);

        _memoryCache.Set(cacheKey, token, tokenExpiresAtUtc);
    }

    private string GetCacheKey(OidcConfidentialClientConfiguration key)
    {
        return $"oidc/{key.TokenEndpoint}/{key.ClientId}";
    }
}
