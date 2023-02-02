using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Common
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class SelectedInvoice
    {
        [JsonPropertyName("$ref")]
        public string? Reference { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice? Invoice { get; set; }
    }
}
