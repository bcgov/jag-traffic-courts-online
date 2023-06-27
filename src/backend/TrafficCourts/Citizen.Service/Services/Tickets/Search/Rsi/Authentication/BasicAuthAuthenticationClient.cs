namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

/// <summary>
/// Gets access token by using basic authentication to the authentication endpoint.
/// </summary>
public class BasicAuthAuthenticationClient : AuthenticationClient<BasicAuthAuthenticationClient>
{
    public BasicAuthAuthenticationClient(HttpClient httpClient, RsiServiceOptions options, ILogger<BasicAuthAuthenticationClient> logger)
        : base(httpClient, options, logger)
    {
    }

    protected override HttpRequestMessage CreateRequest()
    {
        var data = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"}
            };

        var content = new FormUrlEncodedContent(data);
        var request = new HttpRequestMessage(HttpMethod.Post, _options.AuthenticationUrl) { Content = content };
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(_options.ClientId, _options.ClientSecret);

        return request;
    }
}
