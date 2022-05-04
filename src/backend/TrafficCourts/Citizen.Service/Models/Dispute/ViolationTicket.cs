using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class ViolationTicket
    {
        public string? TicketNumber { get; set; }
        public string? Surname { get; set; }
        public string? GivenNames { get; set; }
        public string? DriversLicenceNumber { get; set; }
        public string? DriversLicenceProvince { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        [SwaggerSchema(Format = "date")]
        public DateOnly? Birthdate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? EnforcementOrganization { get; set; }
        public IList<TicketCount> ViolationTicketCounts { get; set; } = new List<TicketCount>();
    }

    public class TicketCount
    {
        public int Count { get; set; } = 0;
        public string Description { get; set; } = String.Empty;
        public string FullSection { get; set; } = String.Empty;
        public string Act { get; set; } = "MVA";
        public double TicketedAmount { get; set; } = Double.NaN;
        public bool IsAct { get; set; } = true;
        public bool IsRegulation { get; set; }
    }
}
