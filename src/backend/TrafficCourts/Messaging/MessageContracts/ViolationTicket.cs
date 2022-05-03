using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class ViolationTicket
    {
        public string? OcrViolationTicket { get; set; }
        public string? TicketNumber { get; set; }
        public string? Surname { get; set; }
        public string? GivenNames { get; set; }
        public string? DriversLicenceNumber { get; set; }
        public string? DriversLicenceProvince { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? EnforcementOrganization { get; set; }
        public IList<TicketCount> TicketCounts { get; set; } = new List<TicketCount>();
    }

    public class TicketCount
    {
        public int Count { get; set; } = 0;
        public string FullSection { get; set; } = String.Empty;
        public double Amount { get; set; } = Double.NaN;
    }
}
