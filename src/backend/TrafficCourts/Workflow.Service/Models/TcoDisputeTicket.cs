using System.Text;

namespace TrafficCourts.Workflow.Service.Models
{
    public class TcoDisputeTicket
    {
        public string CitizenName { get; set; }
        public DateTime TicketIssuanceDate { get; set; }
        public string TicketFileNumber { get; set; }
        public string IssuingOrganization { get; set; }
        public string IssuingLocation { get; set; }
        public string DriversLicense { get; set; }
        public List<TicketDetails> TicketDetails { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Email { get; set; }
        public Dictionary<string, DisputeDetails>[]? DisputeDetails { get; set; }

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

    public class TicketDetails
    {
        public string Section { get; set; }
        public string Subsection { get; set; }
        public string Paragraph { get; set; }
        public string Act { get; set; }
        public double Amount { get; set; }
    }

    public class DisputeDetails
    {
        public string? DisputeType { get; set; }
        public string? DisputeReason { get; set; }
    }
}
