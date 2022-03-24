
using System.Text;

namespace TrafficCourts.Workflow.Service.Models
{
    public class Dispute
    {
        public string TicketNumber { get; set; }
        public string CourtLocation { get; set; }
        public DateTime ViolationDate { get; set; }
        public string DisputantSurname { get; set; }
        public string GivenNames { get; set; }
        public string StreetAddress { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string DriversLicense { get; set; }
        public string DriversLicenseProvince { get; set; }
        public string WorkPhone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EnforcementOrganization { get; set; }
        public DateTime ServiceDate { get; set; }
        public List<TicketCount> TicketCounts { get; set; }
        public bool LawyerRepresentation { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool WitnessIntent { get; set; }

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
