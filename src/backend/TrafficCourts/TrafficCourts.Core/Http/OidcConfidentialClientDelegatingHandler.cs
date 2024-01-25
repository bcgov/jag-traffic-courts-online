using Microsoft.Extensions.Logging;
using System.Net;
using TrafficCourts.Http;

namespace TrafficCourts.Core.Http;

public partial class OidcConfidentialClientDelegatingHandler : DelegatingHandler
{
    private readonly OidcConfidentialClientConfiguration _configuration;
    private readonly ITokenCache _cache;
    private readonly ILogger<OidcConfidentialClientDelegatingHandler> _logger;

    public OidcConfidentialClientDelegatingHandler(OidcConfidentialClientConfiguration configuration, ITokenCache cache, ILogger<OidcConfidentialClientDelegatingHandler> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = GetAccessTokenAsync();

        if (accessToken is null)
        {
            LogGetAccessTokenFailed();

            // could not load access token, bail early without calling real service
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                RequestMessage = request
            };

            return response;
        }

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private string? GetAccessTokenAsync()
    {
        LogGetAccessTokenFromCache();

        var token = _cache.GetToken(_configuration);
        return token?.AccessToken;
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Unable to get access token for service, returning Unauthorized error")]
    public partial void LogGetAccessTokenFailed();

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Getting access token from cache")]
    public partial void LogGetAccessTokenFromCache();
}
