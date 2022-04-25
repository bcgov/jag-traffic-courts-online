using Microsoft.Extensions.Caching.Memory;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public class TokenCache : ITokenCache
{
    private const string CacheKey = "Token";

    private readonly IMemoryCache _cache;

    public TokenCache(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public Token? GetToken()
    {
        if (_cache.TryGetValue(CacheKey, out Token token))
        {
            return token;
        }

        // 

        return null;
    }

    public void SaveToken(Token token)
    {
        DateTimeOffset expires = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - 2);
        _cache.Set(CacheKey, token, expires);
    }
}
