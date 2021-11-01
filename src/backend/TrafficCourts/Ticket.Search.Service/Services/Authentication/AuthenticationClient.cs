using TrafficCourts.Ticket.Search.Service.Configuration;

namespace TrafficCourts.Ticket.Search.Service.Services.Authentication
{
    public class AuthenticationClient : IAuthenticationClient
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceCredentials _credentials;
        private readonly ILogger<AuthenticationClient> _logger;

        public AuthenticationClient(HttpClient httpClient, ServiceCredentials credentials, ILogger<AuthenticationClient> logger)
        {
            _httpClient = httpClient;
            _credentials = credentials;
            _logger = logger;
        }

        public async Task<Token> GetToken(CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, string>
            {
                {"resource", _credentials.ResourceUrl},
                {"client_id", _credentials.ClientId},
                {"client_secret", _credentials.ClientSecret},
                {"scope", "openid"},
                {"response_mode", "form_post"},
                {"grant_type", "client_credentials"}
            };

            var content = new FormUrlEncodedContent(data);
            using var request = new HttpRequestMessage(HttpMethod.Post, _credentials.AuthenticationUrl) { Content = content };

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Get OAuth token failed. {responseData}", responseData);

                throw new AuthenticationException($"The HTTP status code of the response was not expected ({response.StatusCode})", response.StatusCode);
            }

            _logger.LogDebug("get oauth token successfully");
            var token = await response.Content.ReadFromJsonAsync<Token>(cancellationToken: cancellationToken);
            if (token == null)
            {
                throw new AuthenticationException($"Parsing access token failed", response.StatusCode);
            }

            return token;
        }
    }
}
