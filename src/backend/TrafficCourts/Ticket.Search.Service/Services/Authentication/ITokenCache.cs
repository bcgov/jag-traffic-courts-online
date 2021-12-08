namespace TrafficCourts.Ticket.Search.Service.Services.Authentication
{
    public interface ITokenCache
    {
        Token? GetToken();
        void SaveToken(Token token);
    }
}