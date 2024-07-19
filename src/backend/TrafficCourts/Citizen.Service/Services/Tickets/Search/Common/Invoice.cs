using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Common
{
    [ExcludeFromCodeCoverage(Justification = "Poco")]
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

        /// <summary>
        /// The party's surname.
        /// </summary>
        [JsonPropertyName("party_surname")]
        public string? PartySurname { get; set; }

        /// <summary>
        /// The party's first given name.
        /// </summary>
        [JsonPropertyName("party_given_name1")]
        public string? PartyFirstGivenName { get; set; }

        /// <summary>
        /// The party's second given name.
        /// </summary>
        [JsonPropertyName("party_given_name2")]
        public string? PartySecondGivenName { get; set; }


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

        [JsonPropertyName("evt_form_number")]
        public string? FormNumber { get; set; }

        [JsonPropertyName("act")]
        public string? Act { get; set; }

        [JsonPropertyName("section_number")]
        public string? Section { get; set; }
    }
}
