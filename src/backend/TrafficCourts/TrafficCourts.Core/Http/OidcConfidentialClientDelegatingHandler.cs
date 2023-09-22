using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Core.Http;

public class OidcConfidentialClientDelegatingHandler : DelegatingHandler
{
    private readonly IOidcConfidentialClient _client;

    public OidcConfidentialClientDelegatingHandler(IOidcConfidentialClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);

        if (accessToken is not null)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        Token? token = await _client.RequestAccessTokenAsync(cancellationToken);
        return token?.AccessToken;
    }
}
