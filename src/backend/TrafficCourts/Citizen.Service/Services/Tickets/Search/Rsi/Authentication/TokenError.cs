using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public class TokenError
{
    [JsonPropertyName("error")]
    public string? Code { get; set; }

    [JsonPropertyName("error_description")]
    public string? Description { get; set; }
}
