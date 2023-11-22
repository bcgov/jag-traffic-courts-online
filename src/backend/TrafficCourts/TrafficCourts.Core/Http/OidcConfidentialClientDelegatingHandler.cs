using TrafficCourts.Http;

namespace TrafficCourts.Core.Http;

public class OidcConfidentialClientDelegatingHandler : DelegatingHandler
{
    private readonly OidcConfidentialClientConfiguration _configuration;
    private readonly ITokenCache _cache;

    public OidcConfidentialClientDelegatingHandler(OidcConfidentialClientConfiguration configuration, ITokenCache cache)
    {
        _configuration = configuration;
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = GetAccessTokenAsync(cancellationToken);

        if (accessToken is not null)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private string? GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var token = _cache.GetToken(_configuration);
        return token?.AccessToken;
    }
}
