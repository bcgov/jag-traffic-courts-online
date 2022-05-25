using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public class TcoDisputeTicket
    {
        [JsonProperty("citizen_name"), JsonRequired]
        public string CitizenName { get; set; } = String.Empty;
        [JsonProperty("ticket_issuance_date"), JsonRequired]
        public DateTime TicketIssuanceDate { get; set; }
        [JsonProperty("ticket_file_number"), JsonRequired]
        public string TicketFileNumber { get; set; } = String.Empty;
        [JsonProperty("issuing_organization"), JsonRequired]
        public string IssuingOrganization { get; set; } = String.Empty;
        [JsonProperty("issuing_location"), JsonRequired]
        public string IssuingLocation { get; set; } = String.Empty;
        [JsonProperty("drivers_licence"), JsonRequired]
        public string DriversLicence { get; set; } = String.Empty;
        [JsonProperty("ticket_counts"), JsonRequired]
        public List<TicketCount> TicketDetails { set; get; } = new();
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
        public IList<DisputeCount>? DisputeCounts { get; set; }
    }

    public class TicketCount
    {
        [JsonRequired]
        public int Count { get; set; } = 0;
        [JsonRequired]
        public string FullSection { get; set; } = String.Empty;
        public string? Section { get; set; }
        public string? Subsection { get; set; }
        public string? Paragraph { get; set; }
        [JsonRequired]
        public string Act { get; set; } = String.Empty;
        [JsonRequired]
        public double Amount { get; set; }
    }

    public partial class DisputeCount
    {
        [JsonRequired]
        public int Count { get; set; }
        [JsonProperty("dispute_type")]
        public string? DisputeType { get; set; }
    }

}
