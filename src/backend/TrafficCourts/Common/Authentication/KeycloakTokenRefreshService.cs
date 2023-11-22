using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace TrafficCourts.Common.Authentication;

public class KeycloakTokenRefreshService : TokenRefreshService<KeycloakTokenRefreshService>
{
    public KeycloakTokenRefreshService(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        TimeProvider timeProvider, 
        IMemoryCache memoryCache, 
        OidcConfidentialClientConfiguration configuration, 
        ILogger<KeycloakTokenRefreshService> logger) : base(httpClientFactory, httpClientName, timeProvider, memoryCache, configuration, logger)
    {
    }
}
