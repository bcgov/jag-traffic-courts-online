using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Common
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Item
    {
        [JsonPropertyName("selected_invoice")]
        public SelectedInvoice? SelectedInvoice { get; set; }

        [JsonPropertyName("open_invoices_for_site")]
        public object? OpenInvoicesForSite { get; set; }
    }
}
