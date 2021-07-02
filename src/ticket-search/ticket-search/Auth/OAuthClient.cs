using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Auth
{
    public interface IOAuthClient
    {
        public Task<Token> GetRefreshToken(CancellationToken cancellationToken);
    }
    public class OAuthClient : IOAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly OAuthOptions _oAuthOptions;
        private readonly ILogger<OAuthClient> _logger;
        public OAuthClient(HttpClient httpClient, IOptionsMonitor<OAuthOptions> oAuthOptions, ILogger<OAuthClient> logger)
        {
            _httpClient = httpClient;
            _oAuthOptions = oAuthOptions.CurrentValue;
            _logger = logger;
        }

        public async Task<Token> GetRefreshToken(CancellationToken cancellationToken)
        {
            var data = new Dictionary<string, string>
            {
                {"resource", _oAuthOptions.ResourceUrl},
                {"client_id", _oAuthOptions.ClientId},
                {"client_secret", _oAuthOptions.Secret},
                {"scope", "openid"},
                {"response_mode", "form_post"},
                {"grant_type", "client_credentials"}
            };

            var content = new FormUrlEncodedContent(data);
            using var request = new HttpRequestMessage(HttpMethod.Post, _oAuthOptions.OAuthUrl) { Content = content };
            request.Headers.Add("return-client-request-id", "true");
            request.Headers.Add("Accept", "application/json");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("get oauth token failed. {responseData}", responseData); 
                throw new OAuthException(
                    "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
                    (int)response.StatusCode, responseData,
                    response.Headers.ToDictionary(x => x.Key, x => x.Value), null);
            }

            _logger.LogInformation("get oauth token successfully");
            return await response.Content.ReadFromJsonAsync<Token>(cancellationToken:cancellationToken);
        }
    }
}
