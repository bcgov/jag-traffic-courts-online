using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public interface SubmitDispute : IMessage
    {
        string TicketNumber { get; set; }
        string CourtLocation { get; set; }
        DateTime ViolationDate { get; set; }
        string DisputantSurname { get; set; }
        string GivenNames { get; set; }
        string StreetAddress { get; set; }
        string Province { get; set; }
        string PostalCode { get; set; }
        string HomePhone { get; set; }
        string DriversLicence { get; set; }
        string DriversLicenceProvince { get; set; }
        string WorkPhone { get; set; }
        DateTime DateOfBirth { get; set; }
        string EnforcementOrganization { get; set; }
        DateTime ServiceDate { get; set; }
        List<TicketCount> TicketCounts { get; set; }
        bool LawyerRepresentation { get; set; }
        string InterpreterLanguage { get; set; }
        bool WitnessIntent { get; set; }
    }
}
