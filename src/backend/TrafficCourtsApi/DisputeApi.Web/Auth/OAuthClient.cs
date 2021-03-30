using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Auth
{
    public interface IOAuthClient
    {
        Task<Token> GetRefreshToken(CancellationToken cancellationToken);
    }
    public class OAuthClient : IOAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly OAuthOptions _oAuthOptions;
        public OAuthClient(HttpClient httpClient, IOptionsMonitor<OAuthOptions> oAuthOptions)
        {
            _httpClient = httpClient;
            _oAuthOptions = oAuthOptions.CurrentValue;
        }

        public async Task<Token> GetRefreshToken(CancellationToken cancellationToken)
        {
            _httpClient.DefaultRequestHeaders.Add("return-client-request-id", "true");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

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
            using (var request = new HttpRequestMessage(HttpMethod.Post, _oAuthOptions.OAuthUrl) { Content = content })
            {
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var responseData = response.Content == null
                        ? null
                        : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new OAuthException(
                        "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
                        (int)response.StatusCode, responseData,
                        response.Headers.ToDictionary(x => x.Key, x => x.Value), null);
                }


                var stream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<Token>(stream);
            }
        }
    }
}
