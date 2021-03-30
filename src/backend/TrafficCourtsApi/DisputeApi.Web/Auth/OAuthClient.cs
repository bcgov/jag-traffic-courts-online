using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        public OAuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<Token> GetRefreshToken(CancellationToken cancellationToken)
        {
            _httpClient.DefaultRequestHeaders.Add("return-client-request-id", "true");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var data = new Dictionary<string, string>
            {
                {"resource", "https://wsgw.test.jag.gov.bc.ca:8443/paybc"},
                {"client_id", "bd4fe7e3-449d-49be-a680-c12d433004be"},
                {"client_secret", "bc723590-0999-4690-84f6-6534bf333b99"},
                {"scope", "openid"},
                {"response_mode", "form_post"},
                {"grant_type", "client_credentials"}
            };

            var content = new FormUrlEncodedContent(data);
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://wsgw.test.jag.gov.bc.ca/auth/oauth/v2/token") { Content = content })
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

                using (StreamReader sr = new StreamReader(stream))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<Token>(reader);
                }
            }
        }
    }
}
