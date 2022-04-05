using System.Text;

namespace TrafficCourts.Workflow.Service.Models
{
    public class TcoDisputeTicket
    {
        public string CitizenName { get; set; } = null!;
        public DateTime TicketIssuanceDate { get; set; }
        public string TicketFileNumber { get; set; } = null!;
        public string IssuingOrganization { get; set; } = null!;
        public string IssuingLocation { get; set; } = null!;
        public string DriversLicence { get; set; } = null!;
        public List<TicketDetails> TicketDetails { get; set; } = null!;
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
        public string Section { get; set; } = null!;
        public string Subsection { get; set; } = null!;
        public string Paragraph { get; set; } = null!;
        public string Act { get; set; } = null!;
        public double Amount { get; set; }
    }

    public class DisputeDetails
    {
        public string? DisputeType { get; set; }
        public string? DisputeReason { get; set; }
    }
}
