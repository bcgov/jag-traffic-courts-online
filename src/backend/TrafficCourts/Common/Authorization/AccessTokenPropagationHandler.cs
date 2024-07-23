using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace TrafficCourts.Common.Authorization;

public partial class AccessTokenPropagationHandler(
    IHttpContextAccessor httpContextAccessor, 
    IOptions<KeycloakAuthorizationOptions> options,
    ILogger<AccessTokenPropagationHandler> logger) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly KeycloakAuthorizationOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<AccessTokenPropagationHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpContext? context = _httpContextAccessor.HttpContext;

        if (context is null)
        {
            LogHttpContextIsNull();
            return await base.SendAsync(request, cancellationToken);
        }

        var token = await context.GetTokenAsync(_options.RequiredScheme, _options.TokenName);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                _options.RequiredScheme,
                token);
        }
        else
        {
            LogTokenIsEmpty();
        }

        return await base.SendAsync(request, cancellationToken);
    }

    [LoggerMessage(100, LogLevel.Debug, "HttpContext is null, continuing without token")]
    private partial void LogHttpContextIsNull();

    [LoggerMessage(101, LogLevel.Information, "Token is null or empty, continuing without token")]
    private partial void LogTokenIsEmpty();
}
