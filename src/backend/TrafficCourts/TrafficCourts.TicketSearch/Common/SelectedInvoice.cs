using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.TicketSearch.Common
{
    [ExcludeFromCodeCoverage(Justification = "POCO")]
    public class SelectedInvoice
    {
        [JsonPropertyName("$ref")]
        public string? Reference { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice? Invoice { get; set; }
    }
}
