using System.Text.Json.Serialization;

namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public class TcoDisputeTicket
    {
        [JsonPropertyName("citizen_name")]
        public string CitizenName { get; set; }
        [JsonPropertyName("ticket_issuance_date")]
        public DateTime TicketIssuanceDate { get; set; }
        [JsonPropertyName("ticket_file_number")]
        public string TicketFileNumber { get; set; }
        [JsonPropertyName("issuing_organization")]
        public string IssuingOrganization { get; set; }
        [JsonPropertyName("issuing_location")]
        public string IssuingLocation { get; set; }
        [JsonPropertyName("drivers_license")]
        public int DriversLicense { get; set; }
        [JsonPropertyName("ticket_counts")]
        public List<TicketDetails> TicketDetails { set; get; }
        [JsonPropertyName("street_address")]
        public string? StreetAddress { get; set; }
        [JsonPropertyName("city")]
        public string? City { get; set; }
        [JsonPropertyName("province")]
        public string? Province { get; set; }
        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("dispute_counts")]
        public Dictionary<string, DisputeDetails>[]? DisputeDetails { get; set; }
    }

    public class TicketDetails
    {
        public string section { get; set; }
        public string subsection { get; set; }
        public string paragraph { get; set; }
        public string act { get; set; }
        public double amount { get; set; }
    }

    public partial class DisputeDetails
    {
        [JsonPropertyName("dispute_type")]
        public string DisputeType { get; set; }

        [JsonPropertyName("dispute_reason")]
        public string DisputeReason { get; set; }
    }

}
