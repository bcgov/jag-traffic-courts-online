using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.TicketSearch.Common
{
    [ExcludeFromCodeCoverage(Justification = "Poco")]
    public class Invoice
    {
        /// <summary>
        /// The ticket number concatenated with the count number.
        /// </summary>
        [JsonPropertyName("invoice_number")]
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// Not used. Always "10006".
        /// </summary>
        [JsonPropertyName("pbc_ref_number")]
        public string? PbcRefNumber { get; set; }

        /// <summary>
        /// Not used. Always "n/a".
        /// </summary>
        [JsonPropertyName("party_number")]
        public string? PartyNumber { get; set; }

        /// <summary>
        /// Not used. Always "n/a".
        /// </summary>
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

        /// <summary>
        /// Not used. Always "n/a".
        /// </summary>
        [JsonPropertyName("account_number")]
        public string? AccountNumber { get; set; }

        /// <summary>
        /// Not used. Always "0".
        /// </summary>
        [JsonPropertyName("site_number")]
        public string? SiteNumber { get; set; }

        /// <summary>
        /// Not used. Always "Electronic Violation Ticket".
        /// </summary>
        [JsonPropertyName("cust_trx_type")]
        public string? InvoiceType { get; set; }

        /// <summary>
        /// 2024-07-08T09:09
        /// </summary>
        [JsonPropertyName("term_due_date")]
        public string? ViolationDateTime { get; set; }

        /// <summary>
        /// The total amount.
        /// </summary>
        [JsonPropertyName("total")]
        public decimal TicketedAmount { get; set; }

        /// <summary>
        /// The amount currently due. This is the total amount minus $25 if during the discount period.
        /// </summary>
        [JsonPropertyName("amount_due")]
        public decimal AmountDue { get; set; }

        /// <summary>
        /// The offense description.
        /// </summary>
        [JsonPropertyName("attribute1")]
        public string? OffenceDescription { get; set; }

        /// <summary>
        /// Not used.
        /// </summary>
        [JsonPropertyName("attribute2")]
        public string? VehicleDescription { get; set; }

        /// <summary>
        /// The date of the violation for example 2024-07-08.
        /// </summary>
        [JsonPropertyName("attribute3")]
        public string? ViolationDate { get; set; }

        /// <summary>
        /// The fine amount less the discount if applicable. For example, "84.00".
        /// </summary>
        [JsonPropertyName("attribute4")]
        public string? DiscountAmount { get; set; }

        /// <summary>
        /// The event form number. For example, "MV6000E (040924)"
        /// </summary>
        [JsonPropertyName("evt_form_number")]
        public string? FormNumber { get; set; }

        /// <summary>
        /// The act or regulation that was violated. MVR or MVA.
        /// </summary>
        [JsonPropertyName("act")]
        public string? Act { get; set; }

        /// <summary>
        /// The formatted section number. For example, "4.03(1)".
        /// </summary>
        [JsonPropertyName("section_number")]
        public string? Section { get; set; }
    }
}
