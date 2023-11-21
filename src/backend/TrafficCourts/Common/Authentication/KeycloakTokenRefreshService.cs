using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace TrafficCourts.Common.Authentication;

public class KeycloakTokenRefreshService : TokenRefreshService
{
    public KeycloakTokenRefreshService(IMemoryCache memoryCache, OidcConfidentialClientConfiguration configuration, ILogger<KeycloakTokenRefreshService> logger)
        : base(memoryCache, configuration, logger)
    {
    }
}
