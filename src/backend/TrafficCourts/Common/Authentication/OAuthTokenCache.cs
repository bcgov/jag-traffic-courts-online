using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text;

namespace TrafficCourts.Common.Authentication;

public class OAuthTokenCache : TokenCache<OAuthOptions, Token>
{
    public OAuthTokenCache(IMemoryCache memoryCache, ILogger<OAuthTokenCache> logger) : base(memoryCache, logger)
    {
    }

    protected override string GetCacheKey(OAuthOptions configuration)
    {
        StringBuilder buffer = new StringBuilder();

        // create a cache key based on all the parameters
        buffer.Append(configuration.TokenUri);
        buffer.Append('|');
        buffer.Append(configuration.ClientId);
        buffer.Append('|');
        buffer.Append(configuration.ClientSecret);
        buffer.Append('|');

        return buffer.ToString();
    }
}