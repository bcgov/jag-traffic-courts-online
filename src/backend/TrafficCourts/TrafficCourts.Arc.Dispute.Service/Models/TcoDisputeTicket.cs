using Newtonsoft.Json;

namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public class TcoDisputeTicket
    {
        [JsonProperty("citizen_surname")]
        [JsonRequired]
        public string? Surname { get; set; } = String.Empty;

        [JsonProperty("citizen_given_name_1")]
        public string? GivenName1 { get; set; }

        [JsonProperty("citizen_given_name_2")]
        public string? GivenName2 { get; set; }

        [JsonProperty("citizen_given_name_3")]
        public string? GivenName3 { get; set; }

        [JsonProperty("ticket_issuance_date")]
        [JsonRequired]
        public DateTime TicketIssuanceDate { get; set; }

        [JsonProperty("ticket_file_number")]
        [JsonRequired]
        public string TicketFileNumber { get; set; } = String.Empty;

        [JsonProperty("issuing_organization")]
        public string IssuingOrganization { get; set; } = String.Empty;
        
        [JsonProperty("issuing_location")]
        [JsonRequired]
        public string IssuingLocation { get; set; } = String.Empty;
        
        [JsonProperty("drivers_licence")]
        [JsonRequired]
        public string DriversLicence { get; set; } = String.Empty;
        
        [JsonProperty("ticket_counts")]
        [JsonRequired]
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
        public List<DisputeCount> DisputeCounts { get; set; } = new();
    }

    public class TicketCount
    {
        [JsonProperty("count")]
        [JsonRequired]
        public int Count { get; set; } = 0;

        [Obsolete("No used in ARC file")]
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
        
        [JsonProperty("amount")]
        [JsonRequired]
        public decimal Amount { get; set; }
    }

    public partial class DisputeCount
    {
        [JsonProperty("count")]
        [JsonRequired]
        public int Count { get; set; }

        [JsonProperty("dispute_type")]
        public string? DisputeType { get; set; }
    }
}
