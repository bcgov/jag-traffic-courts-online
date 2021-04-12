using DisputeApi.Web.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.TicketLookup
{
    public interface IRsiRestApi
    {
        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/")]
        Task<RawTicketSearchResponse> GetTicket(GetTicketParams ticketParams, CancellationToken cancellationToken);

        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/{invoiceNumber}")]
        Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken);
    }

    public class GetTicketParams
    {
        [AliasAs("in")]
        public string TicketNumber { get; set; }

        [AliasAs("prn")]
        public string Prn { get; set; }

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
        public string InvoiceNumber { get; set; }

        [JsonPropertyName("pbc_ref_number")]
        public string PbcRefNumber { get; set; }

        [JsonPropertyName("party_number")]
        public string PartyNumber { get; set; }

        [JsonPropertyName("party_name")]
        public string PartyName { get; set; }

        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("site_number")]
        public string SiteNumber { get; set; }

        [JsonPropertyName("cust_trx_type")]
        public string CustTrxType { get; set; }

        [JsonPropertyName("term_due_date")]
        public string TermDueDate { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("amount_due")]
        public decimal AmountDue { get; set; }

        [JsonPropertyName("attribute1")]
        public string Attribute1 { get; set; }

        [JsonPropertyName("attribute2")]
        public string Attribute2 { get; set; }

        [JsonPropertyName("attribute3")]
        public string Attribute3 { get; set; }

        [JsonPropertyName("attribute4")]
        public string Attribute4 { get; set; }
    }

    public static class RsiRawResponseExtensions
    {
        public static TicketDispute ConvertToTicketDispute(this RawTicketSearchResponse rawResponse)
        {
            if (rawResponse.Items == null || rawResponse.Items.Count <= 0) return null;
            TicketDispute ticketDispute = new TicketDispute {RawResponse = rawResponse};
            Invoice firstInvoice = rawResponse.Items.First().SelectedInvoice.Invoice;
            ticketDispute.ViolationTicketNumber = firstInvoice.InvoiceNumber.Remove(firstInvoice.InvoiceNumber.Length - 1);
            ticketDispute.ViolationDate = DateTime.Parse(firstInvoice.TermDueDate).ToString("yyyy-MM-dd");
            ticketDispute.ViolationTime = DateTime.Parse(firstInvoice.TermDueDate).ToString("HH:mm");
            ticketDispute.Offences = rawResponse.Items.Select((_, i) => new Offence
            {
                OffenceNumber = i + 1,
                AmountDue=_.SelectedInvoice.Invoice.AmountDue,
                Description = _.SelectedInvoice.Invoice.Attribute1,
                DueDate = _.SelectedInvoice.Invoice.TermDueDate,
                TicketAmount=_.SelectedInvoice.Invoice.Total
            }).ToList();
            return ticketDispute;
        }
    }
#endregion
}
