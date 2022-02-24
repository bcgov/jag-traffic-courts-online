using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using TrafficCourts.Ticket.Search.Service.Configuration;

namespace TrafficCourts.Ticket.Search.Service.Authentication
{
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
        private readonly ILogger<AuthenticationHandler> _logger;

        public AuthenticationHandler(ITokenCache tokenCache, ILogger<AuthenticationHandler> logger)
        {
            _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Token? token = _tokenCache.GetToken();
            if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            {
                _logger.LogTrace("Using {Token}", token);

                // in testing, this access token is not a bearer token but more of an API key even though 
                // the TokenType is returned as "Bearer"
                ////request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                request.Headers.Authorization = new AuthenticationHeaderValue(token.AccessToken);
            }
            else
            {
                _logger.LogInformation("No authentication token available");
            }

            return await base.SendAsync(request, cancellationToken);
        }

    }

    public class Token
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        /// <summary>
        /// The number of seconds after the token was issued that it expires.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }

    public class TokenError
    {
        [JsonPropertyName("error")]
        public string? Code { get; set; }

        [JsonPropertyName("error_description")]
        public string? Description { get; set; }
    }

    public interface ITokenCache
    {
        Token? GetToken();
        void SaveToken(Token token);
    }

    public class TokenCache : ITokenCache
    {
        private const string CacheKey = "Token";

        private readonly IMemoryCache _cache;

        public TokenCache(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public Token? GetToken()
        {
            if (_cache.TryGetValue(CacheKey, out Token token))
            {
                return token;
            }

            // 

            return null;
        }

        public void SaveToken(Token token)
        {
            DateTimeOffset expires = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - 2);
            _cache.Set(CacheKey, token, expires);
        }
    }

    [Serializable]
    public class AuthenticationException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public AuthenticationException(
            string? message,
            HttpStatusCode statusCode,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
