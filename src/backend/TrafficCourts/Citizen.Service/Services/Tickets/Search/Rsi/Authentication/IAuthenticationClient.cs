namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public interface IAuthenticationClient
{
    /// <summary>
    /// Gets an access token.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Token> GetTokenAsync(CancellationToken cancellationToken = default);
}
