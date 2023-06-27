using System.Net.Http.Headers;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
{
    public BasicAuthenticationHeaderValue(string username, string password)
        : base("Basic", Encode(username, password))
    {
    }

    private static string Encode(string username, string password)
    {
        var credentials = $"{username}:{password}";
        var encoded = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));
        return encoded;
    }
}
