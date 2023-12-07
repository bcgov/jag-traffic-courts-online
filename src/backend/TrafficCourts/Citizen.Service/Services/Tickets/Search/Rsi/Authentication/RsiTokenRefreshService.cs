using Microsoft.Extensions.Caching.Memory;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public class RsiTokenRefreshService : TokenRefreshService<RsiTokenRefreshService>
{
    public RsiTokenRefreshService(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        TimeProvider timeProvider,
        IMemoryCache memoryCache,
        OidcConfidentialClientConfiguration configuration,
        ILogger<RsiTokenRefreshService> logger) : base(httpClientFactory, httpClientName, timeProvider, memoryCache, configuration, logger)
    {
    }
}
