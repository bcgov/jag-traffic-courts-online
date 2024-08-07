using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TrafficCourts.TicketSearch.Common;

namespace TrafficCourts.TicketSearch.Rsi
{
    [ExcludeFromCodeCoverage(Justification = "POCO")]
    public class RawTicketSearchResponse
    {
        [JsonPropertyName("items")]
        public List<Item>? Items { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

}
