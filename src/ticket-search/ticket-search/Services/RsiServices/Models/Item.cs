using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Services.RsiServices.Models
{
    public class Item
    {
        [JsonPropertyName("selected_invoice")]
        public SelectedInvoice SelectedInvoice { get; set; }

        [JsonPropertyName("open_invoices_for_site")]
        public object OpenInvoicesForSite { get; set; }
    }
}
