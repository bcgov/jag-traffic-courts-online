namespace TrafficCourts.Ticket.Search.Service.Services.Authentication
{
    public interface IAuthenticationClient
    {
        /// <summary>
        /// Gets an access token.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Token> GetToken(CancellationToken cancellationToken = default);
    }
}
