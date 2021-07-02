using System.Text.Json.Serialization;

namespace Gov.TicketSearch.Services.RsiServices.Models
{
    public class SelectedInvoice
    {
        [JsonPropertyName("$ref")]
        public string Reference { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice Invoice { get; set; }
    }
}