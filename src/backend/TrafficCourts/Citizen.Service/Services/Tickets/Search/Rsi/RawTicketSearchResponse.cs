using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class RawTicketSearchResponse
    {
        [JsonPropertyName("items")]
        public List<Item>? Items { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

}
