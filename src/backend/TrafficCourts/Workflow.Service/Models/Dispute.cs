
using System.Text;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Utils;

namespace TrafficCourts.Workflow.Service.Models
{
    public class Dispute
    {
        public string TicketNumber { get; set; } = null!;
        public string CourtLocation { get; set; } = null!;
        public DateTime ViolationDate { get; set; }
        public string DisputantSurname { get; set; } = null!;
        public string GivenNames { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string HomePhone { get; set; } = null!;
        public string DriversLicence { get; set; } = null!;
        public string DriversLicenceProvince { get; set; } = null!;
        public string WorkPhone { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string EnforcementOrganization { get; set; } = null!;
        public DateTime ServiceDate { get; set; }
        public List<TicketCount> TicketCounts { get; set; } = null!;
        public bool LawyerRepresentation { get; set; }
        public string InterpreterLanguage { get; set; } = null!;
        public bool WitnessIntent { get; set; }
        public string? OcrViolationTicket { get; set; }

        public override string ToString()
        {
            return GetType().GetProperties()
                .Select(info => (info.Name, Value: info.GetValue(this, null) ?? "(null)"))
                .Aggregate(
                    new StringBuilder(),
                    (sb, pair) => sb.AppendLine($"{pair.Name}: {pair.Value}"),
                    sb => sb.ToString());
        }
    }
}
