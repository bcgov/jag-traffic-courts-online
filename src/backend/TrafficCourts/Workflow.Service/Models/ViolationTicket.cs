
using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Workflow.Service.Models
{
    public class ViolationTicket
    {
        public string? OcrViolationTicket { get; set; }
        public string? TicketNumber { get; set; }
        public string? Surname { get; set; }
        public string? GivenNames { get; set; }
        public string? DriversLicenceNumber { get; set; }
        public string? DriversLicenceProvince { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? EnforcementOrganization { get; set; }
        public IList<ViolationTicketCount> ViolationTicketCounts { get; set; } = new List<ViolationTicketCount>();
    }

    public class ViolationTicketCount
    {
        public int Count { get; set; }
        public string Description { get; set; } = null!;
        public string FullSection { get; set; } = null!;
        public string Act { get; set; } = null!;
        public double TicketedAmount { get; set; } = Double.NaN;
        public bool IsAct { get; set; }
        public bool IsRegulation { get; set; }
    }
}
