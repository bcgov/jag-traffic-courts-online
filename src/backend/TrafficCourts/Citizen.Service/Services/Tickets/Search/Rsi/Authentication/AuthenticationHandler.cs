using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly ITokenCache _tokenCache;
    private readonly ILogger<AuthenticationHandler> _logger;

    public AuthenticationHandler(ITokenCache tokenCache, ILogger<AuthenticationHandler> logger)
    {
        _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting token from cache");

        Token? token = _tokenCache.GetToken();
        if (token != null && !string.IsNullOrEmpty(token.AccessToken))
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Adding bearer authentication header using {Token}", token);
            }
            else
            {
                _logger.LogDebug("Adding bearer authentication header");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }
        else
        {
            _logger.LogInformation("Authentication token not available");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
