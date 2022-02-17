using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public class TcoDisputeTicket
    {
        [JsonProperty("citizen_name")]
        public string CitizenName { get; set; }
        [JsonProperty("ticket_issuance_date")]
        public DateTime TicketIssuanceDate { get; set; }
        [JsonProperty("ticket_file_number")]
        public string TicketFileNumber { get; set; }
        [JsonProperty("issuing_organization")]
        public string IssuingOrganization { get; set; }
        [JsonProperty("issuing_location")]
        public string IssuingLocation { get; set; }
        [JsonProperty("drivers_license")]
        public string DriversLicense { get; set; }
        [JsonProperty("ticket_counts")]
        public List<TicketDetails> TicketDetails { set; get; }
        [JsonProperty("street_address")]
        public string? StreetAddress { get; set; }
        [JsonProperty("city")]
        public string? City { get; set; }
        [JsonProperty("province")]
        public string? Province { get; set; }
        [JsonProperty("postal_code")]
        public string? PostalCode { get; set; }
        [JsonProperty("email")]
        public string? Email { get; set; }
        [JsonProperty("dispute_counts")]
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
        [JsonProperty("dispute_type")]
        public string DisputeType { get; set; }

        [JsonProperty("dispute_reason")]
        public string DisputeReason { get; set; }
    }

}
