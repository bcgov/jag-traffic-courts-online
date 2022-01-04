using System.Net.Http.Headers;
using System.Net.Http.Json;
using TrafficCourts.Ticket.Search.Service.Configuration;

namespace TrafficCourts.Ticket.Search.Service.Services.Authentication
{
    public class AuthenticationClient : IAuthenticationClient
    {
        private readonly HttpClient _httpClient;
        private readonly TicketSearchProperties _ticketSearchProperties;
        private readonly ILogger<AuthenticationClient> _logger;

        public AuthenticationClient(HttpClient httpClient, TicketSearchProperties ticketSearchProperties, ILogger<AuthenticationClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ticketSearchProperties = ticketSearchProperties ?? throw new ArgumentNullException(nameof(ticketSearchProperties));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Token> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, string>
            {
                {"resource", _ticketSearchProperties.ResourceUrl.ToString() },
                {"client_id", _ticketSearchProperties.ClientId},
                {"client_secret", _ticketSearchProperties.ClientSecret},
                {"scope", "openid"},
                {"response_mode", "form_post"},
                {"grant_type", "client_credentials"}
            };

            var content = new FormUrlEncodedContent(data);
            using var request = new HttpRequestMessage(HttpMethod.Post, _ticketSearchProperties.AuthenticationUrl) { Content = content };

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

    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly ITokenCache _tokenCache;

        public AuthenticationHandler(ITokenCache tokenCache)
        {
            _tokenCache = tokenCache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {            
            Token? token = _tokenCache.GetToken();
            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

    }
}
