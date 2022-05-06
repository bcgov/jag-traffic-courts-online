
using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Workflow.Service.Models
{
    public class ViolationTicket
    {
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
        public string? OrganizationLocation { get; set; }
        public IList<ViolationTicketCount> ViolationTicketCounts { get; set; } = new List<ViolationTicketCount>();
    }

    public class ViolationTicketCount
    {
        public short Count { get; set; }
        public string? Description { get; set; }
        public string? FullSection { get; set; }
        public string? ActRegulation { get; set; }
        public float? TicketedAmount { get; set; }
        public bool? IsAct { get; set; }
        public bool? IsRegulation { get; set; }
    }
}
