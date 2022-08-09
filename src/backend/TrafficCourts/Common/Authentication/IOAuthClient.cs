namespace TrafficCourts.Common.Authentication;

public interface IOAuthClient
{
    Task<Token> GetTokenAsync(OAuthOptions options, CancellationToken cancellationToken);
}
