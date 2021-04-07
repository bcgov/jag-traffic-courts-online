using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static DisputeApi.Web.Features.TicketLookup.TicketLookup;

namespace DisputeApi.Web.Features.TicketLookup
{
    public interface IRsiRestApi
    {
        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/")]
        Task<RawTicketSearchResponse> GetTicket(GetTicketParams ticketParams);

        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/{invoiceNumber}")]
        Task<Invoice> GetInvoice(string invoiceNumber);
    }

    public class GetTicketParams
    {
        [AliasAs("in")]
        public string TicketNumber { get; set; }

        [AliasAs("prn")]
        public string PRN { get; set; }

        [AliasAs("time")]
        public string IssuedTime { get; set; }
    }

#region Raw RSI data for troubleshooting
    public class ViolationCount
    {
        [JsonPropertyName("count_number")]
        public int CountNumber { get; set; }

        [JsonPropertyName("ticket_amount")]
        public decimal TicketAmount { get; set; }

        [JsonPropertyName("amount_due")]
        public decimal AmountDue { get; set; }

        [JsonPropertyName("due_date")]
        public string DueDate { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

    }


    public class RawTicketSearchResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("selected_invoice")]
        public SelectedInvoice SelectedInvoice { get; set; }

        [JsonPropertyName("open_invoices_for_site")]
        public object OpenInvoicesForSite { get; set; }
    }

    public class SelectedInvoice
    {
        [JsonPropertyName("$ref")]
        public string Reference { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice Invoice { get; set; }
    }

    public class Invoice
    {
        [JsonPropertyName("invoice_number")]
        public string invoice_number { get; set; }

        [JsonPropertyName("pbc_ref_number")]
        public string pbc_ref_number { get; set; }

        [JsonPropertyName("party_number")]
        public string party_number { get; set; }

        [JsonPropertyName("party_name")]
        public string party_name { get; set; }

        [JsonPropertyName("account_number")]
        public string account_number { get; set; }

        [JsonPropertyName("site_number")]
        public string site_number { get; set; }

        [JsonPropertyName("cust_trx_type")]
        public string cust_trx_type { get; set; }

        [JsonPropertyName("term_due_date")]
        public string term_due_date { get; set; }

        [JsonPropertyName("total")]
        public decimal total { get; set; }

        [JsonPropertyName("amount_due")]
        public decimal amount_due { get; set; }

        [JsonPropertyName("attribute1")]
        public string attribute1 { get; set; }

        [JsonPropertyName("attribute2")]
        public string attribute2 { get; set; }

        [JsonPropertyName("attribute3")]
        public string attribute3 { get; set; }

        [JsonPropertyName("attribute4")]
        public string attribute4 { get; set; }
    }

    public static class RsiRawResponseExtensions
    {
        public static Response ConvertToResponse(this RawTicketSearchResponse rawResponse)
        {
            if (rawResponse.Items == null || rawResponse.Items.Count <= 0) return null;
            Response response = new Response();
            response.RawResponse = rawResponse;
            Invoice firstInvoice = rawResponse.Items.First().SelectedInvoice.Invoice;
            response.ViolationTicketNumber = firstInvoice.invoice_number.Remove(firstInvoice.invoice_number.Length - 1);
            response.ViolationDate = DateTime.Parse(firstInvoice.term_due_date).ToString("yyyy-MM-dd");
            response.ViolationTime = DateTime.Parse(firstInvoice.term_due_date).ToString("HH:mm");
            response.Offences = rawResponse.Items.Select((_, i) => new Offence
            {
                OffenceNumber = i + 1,
                AmountDue=_.SelectedInvoice.Invoice.amount_due,
                Description = _.SelectedInvoice.Invoice.attribute1,
                DueDate = _.SelectedInvoice.Invoice.term_due_date,
                TicketAmount=_.SelectedInvoice.Invoice.total,
                Status = OffenceStatus.OutstandingBalance
            }).ToList();
            return response;
        }
    }
#endregion
}
