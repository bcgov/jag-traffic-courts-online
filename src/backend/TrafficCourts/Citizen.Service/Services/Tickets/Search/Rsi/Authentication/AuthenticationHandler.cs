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

public interface IAuthenticationClient
{
    /// <summary>
    /// Gets an access token.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Token> GetTokenAsync(CancellationToken cancellationToken = default);
}

public class AuthenticationClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;
    //private readonly TicketSearchProperties _ticketSearchProperties;
    private readonly ILogger<AuthenticationClient> _logger;
    private RsiServiceOptions _options;

    public AuthenticationClient(HttpClient httpClient, RsiServiceOptions options, ILogger<AuthenticationClient> logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Token> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, string>
            {
                {"resource", _options.ResourceUrl.ToString() },
                {"client_id", _options.ClientId},
                {"client_secret", _options.ClientSecret},
                {"scope", "openid"},
                {"response_mode", "form_post"},
                {"grant_type", "client_credentials"}
            };

        var content = new FormUrlEncodedContent(data);
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.AuthenticationUrl) { Content = content };

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var exception = await HandleNotSuccessful(response, cancellationToken);
            throw exception;
        }

        _logger.LogDebug("Fetched access token");
        var token = await response.Content.ReadFromJsonAsync<Token>(cancellationToken: cancellationToken);
        if (token == null)
        {
            throw new AuthenticationException($"Parsing JSON access token failed", response.StatusCode);
        }

        return token;
    }

    private async Task<AuthenticationException> HandleNotSuccessful(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // probably { "error":"invalid_client","error_description":"The given client credentials were not valid" }
            TokenError? error = await response.Content.ReadFromJsonAsync<TokenError>(cancellationToken: cancellationToken);

            var scope = new Dictionary<string, object>() { { "HttpStatusCode", response.StatusCode } };
            if (error != null) scope.Add("@Error", error);

            using (_logger.BeginScope(scope))
            {
                _logger.LogError("Authentication failed, check client id and secret");
            }
        }

        return new AuthenticationException($"The HTTP status code of the response was not expected ({response.StatusCode})", response.StatusCode);
    }
}
