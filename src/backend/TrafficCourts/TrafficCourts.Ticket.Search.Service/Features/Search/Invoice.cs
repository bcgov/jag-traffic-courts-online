using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Ticket.Search.Service.Features.Search
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Invoice
    {
        [JsonPropertyName("invoice_number")]
        public string? InvoiceNumber { get; set; }

        [JsonPropertyName("pbc_ref_number")]
        public string? PbcRefNumber { get; set; }

        [JsonPropertyName("party_number")]
        public string? PartyNumber { get; set; }

        [JsonPropertyName("party_name")]
        public string? PartyName { get; set; }

        [JsonPropertyName("account_number")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("site_number")]
        public string? SiteNumber { get; set; }

        [JsonPropertyName("cust_trx_type")]
        public string? InvoiceType { get; set; }

        [JsonPropertyName("term_due_date")]
        public string? ViolationDateTime { get; set; }

        [JsonPropertyName("total")]
        public decimal TicketedAmount { get; set; }

        [JsonPropertyName("amount_due")]
        public decimal AmountDue { get; set; }

        [JsonPropertyName("attribute1")]
        public string? OffenceDescription { get; set; }

        [JsonPropertyName("attribute2")]
        public string? VehicleDescription { get; set; }

        [JsonPropertyName("attribute3")]
        public string? ViolationDate { get; set; }

        [JsonPropertyName("attribute4")]
        public string? DiscountAmount { get; set; }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Item
    {
        [JsonPropertyName("selected_invoice")]
        public SelectedInvoice? SelectedInvoice { get; set; }

        [JsonPropertyName("open_invoices_for_site")]
        public object? OpenInvoicesForSite { get; set; }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class SelectedInvoice
    {
        [JsonPropertyName("$ref")]
        public string? Reference { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice? Invoice { get; set; }
    }
}
