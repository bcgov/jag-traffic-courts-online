using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public class TcoDisputeTicket
    {
        [JsonProperty("citizen_name"), JsonRequired]
        public string CitizenName { get; set; }
        [JsonProperty("ticket_issuance_date"), JsonRequired]
        public DateTime TicketIssuanceDate { get; set; }
        [JsonProperty("ticket_file_number"), JsonRequired]
        public string TicketFileNumber { get; set; }
        [JsonProperty("issuing_organization"), JsonRequired]
        public string IssuingOrganization { get; set; }
        [JsonProperty("issuing_location"), JsonRequired]
        public string IssuingLocation { get; set; }
        [JsonProperty("drivers_licence"), JsonRequired]
        public string DriversLicence { get; set; }
        [JsonProperty("ticket_counts"), JsonRequired]
        public IList<TicketCount> TicketDetails { set; get; }
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
        public int Count { get; set; }
        [JsonRequired]
        public string Section { get; set; }
        [JsonRequired]
        public string Subsection { get; set; }
        [JsonRequired]
        public string Paragraph { get; set; }
        [JsonRequired]
        public string Act { get; set; }
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
