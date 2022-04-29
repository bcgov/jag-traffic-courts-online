using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class DisputeApproved: IMessage
    {
        public string CitizenName { get; set; } = String.Empty;
        public DateTime? TicketIssuanceDate { get; set; }
        public string TicketFileNumber { get; set; } = String.Empty;
        public string IssuingOrganization { get; set; } = String.Empty;
        public string IssuingLocation { get; set; } = String.Empty;
        public string DriversLicence { get; set; } = String.Empty;
        public IList<ViolationTicketCount> ViolationTicketCounts { get; set; } = new List<ViolationTicketCount>();
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Email { get; set; }
        public IList<DisputeCount> DisputeCounts { get; set; } = new List<DisputeCount>();
    }

    public class ViolationTicketCount
    {
        public int Count { get; set; } = 0;
        public string Section { get; set; } = String.Empty;
        public string Subsection { get; set; } = String.Empty;
        public string Paragraph { get; set; } = String.Empty;
        public string Act { get; set; } = String.Empty;
        public double Amount { get; set; } = Double.NaN;
    }

    public class DisputeCount
    {
        public int Count { get; set; } = 0;
        public string? DisputeType { get; set; }
    }
}
