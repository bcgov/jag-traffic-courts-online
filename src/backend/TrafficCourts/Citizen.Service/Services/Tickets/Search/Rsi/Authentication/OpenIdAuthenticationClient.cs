namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

/// <summary>
/// Gets access token by using OpenId client credentials to the authentication endpoint.
/// </summary>
public class OpenIdAuthenticationClient : AuthenticationClient<OpenIdAuthenticationClient>
{
    public OpenIdAuthenticationClient(HttpClient httpClient, RsiServiceOptions options, ILogger<OpenIdAuthenticationClient> logger)
        : base(httpClient, options, logger)
    {
    }

    protected override HttpRequestMessage CreateRequest()
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
        var request = new HttpRequestMessage(HttpMethod.Post, _options.AuthenticationUrl) { Content = content };
        return request;
    }
}
