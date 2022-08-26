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
        [JsonProperty("issuing_organization")]
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
        [JsonRequired, JsonProperty("count")]
        public int Count { get; set; } = 0;
        [JsonProperty("subparagraph")]
        public string? Subparagraph { get; set; }
        [JsonProperty("section")]
        public string Section { get; set; } = String.Empty;
        [JsonProperty("subsection")]
        public string? Subsection { get; set; }
        [JsonProperty("paragraph")]
        public string? Paragraph { get; set; }
        [JsonProperty("act")]
        public string Act { get; set; } = "MVA";
        [JsonRequired, JsonProperty("amount")]
        public double Amount { get; set; }
    }

    public partial class DisputeCount
    {
        [JsonRequired, JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("dispute_type")]
        public string? DisputeType { get; set; }
    }

}
