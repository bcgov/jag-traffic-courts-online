using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public interface DisputeApproved: IMessage
    {
        string CitizenName { get; set; }
        DateTime TicketIssuanceDate { get; set; }
        string TicketFileNumber { get; set; }
        string IssuingOrganization { get; set; }
        string IssuingLocation { get; set; }
        string DriversLicence { get; set; }
        IList<TicketDetails> TicketDetails { get; set; }
        string? StreetAddress { get; set; }
        string? City { get; set; }
        string? Province { get; set; }
        string? PostalCode { get; set; }
        string? Email { get; set; }
        IDictionary<string, DisputeDetails>[]? DisputeDetails { get; set; }
    }

    public interface TicketDetails
    {
        string Section { get; set; }
        string Subsection { get; set; }
        string Paragraph { get; set; }
        string Act { get; set; }
        double Amount { get; set; }
    }

    public interface DisputeDetails
    {
        string? DisputeType { get; set; }
        string? DisputeReason { get; set; }
    }
}
