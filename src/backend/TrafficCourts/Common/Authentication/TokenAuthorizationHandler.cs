using System.Net.Http.Headers;

namespace TrafficCourts.Common.Authentication;

public class TokenAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly OAuthOptions _options;

    public TokenAuthorizationHandler(ITokenService tokenService, OAuthOptions options)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // get the access token and add it to the Authorization header of the request 
        var token = await _tokenService.GetTokenAsync(_options, cancellationToken);

        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
