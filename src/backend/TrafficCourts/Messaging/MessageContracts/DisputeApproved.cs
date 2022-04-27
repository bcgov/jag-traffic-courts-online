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
        public DateTime TicketIssuanceDate { get; set; }
        public string TicketFileNumber { get; set; } = String.Empty;
        public string IssuingOrganization { get; set; } = String.Empty;
        public string IssuingLocation { get; set; } = String.Empty;
        public string DriversLicence { get; set; } = String.Empty;
        public IList<TicketDetails> TicketDetails { get; set; } = new List<TicketDetails>();
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Email { get; set; }
        public IDictionary<string, DisputeDetails>[]? DisputeDetails { get; set; }
    }

    public class TicketDetails
    {
        public string Section { get; set; } = String.Empty;
        public string Subsection { get; set; } = String.Empty;
        public string Paragraph { get; set; } = String.Empty;
        public string Act { get; set; } = String.Empty;
        public double Amount { get; set; } = Double.NaN;
    }

    public class DisputeDetails
    {
        public string? DisputeType { get; set; } 
        public string? DisputeReason { get; set; }
    }
}
