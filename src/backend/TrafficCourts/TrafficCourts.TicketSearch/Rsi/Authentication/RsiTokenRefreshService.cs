﻿using Microsoft.Extensions.Logging;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace TrafficCourts.TicketSearch.Rsi.Authentication;

public class RsiTokenRefreshService : TokenRefreshService<RsiTokenRefreshService>
{
    public RsiTokenRefreshService(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        TimeProvider timeProvider,
        ITokenCache cache,
        OidcConfidentialClientConfiguration configuration,
        ILogger<RsiTokenRefreshService> logger) : base(httpClientFactory, httpClientName, timeProvider, cache, configuration, logger)
    {
    }
}
