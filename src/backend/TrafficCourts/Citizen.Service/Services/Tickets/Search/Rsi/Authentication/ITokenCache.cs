namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public interface ITokenCache
{
    Token? GetToken();
    void SaveToken(Token token);
}
