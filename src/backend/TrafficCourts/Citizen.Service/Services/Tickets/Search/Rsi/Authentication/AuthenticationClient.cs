namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

/// <summary>
/// Base class for getting a bearer access token from the authentication endpoint.
/// </summary>
/// <typeparam name="TClient"></typeparam>
public abstract class AuthenticationClient<TClient> : IAuthenticationClient where TClient : IAuthenticationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TClient> _logger;
    protected readonly RsiServiceOptions _options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected AuthenticationClient(HttpClient httpClient, RsiServiceOptions options, ILogger<TClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Token> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest();
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

    /// <summary>
    /// Create the authentication request.
    /// </summary>
    /// <returns></returns>
    protected abstract HttpRequestMessage CreateRequest();
}
