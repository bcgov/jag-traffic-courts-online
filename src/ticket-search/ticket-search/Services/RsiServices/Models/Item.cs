using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Gov.TicketSearch.Services.RsiServices.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Item
    {
        [JsonPropertyName("selected_invoice")]
        public SelectedInvoice SelectedInvoice { get; set; }

        [JsonPropertyName("open_invoices_for_site")]
        public object OpenInvoicesForSite { get; set; }
    }
}
