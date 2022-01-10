using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Ticket.Search.Service.Features.Search.Rsi
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
