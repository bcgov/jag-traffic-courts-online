using System.Text;

namespace TrafficCourts.Workflow.Service.Models
{
    public class TcoDisputeTicket
    {
        public string CitizenName { get; set; } = null!;
        public DateTime? TicketIssuanceDate { get; set; }
        public string TicketFileNumber { get; set; } = null!;
        public string IssuingOrganization { get; set; } = null!;
        public string IssuingLocation { get; set; } = null!;
        public string DriversLicence { get; set; } = null!;
        public IList<TicketCount> TicketDetails { get; set; } = null!;
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Email { get; set; }
        public IList<DisputeCount> DisputeCounts { get; set; } = new List<DisputeCount>();

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

    public class TicketCount
    {
        public int Count { get; set; } = 0;
        public string FullSection { get; set; } = String.Empty;
        public string? Section { get; set; }
        public string? Subsection { get; set; }
        public string? Paragraph { get; set; }
        public string Act { get; set; } = String.Empty;
        public double? Amount { get; set; }
    }

    public class DisputeCount
    {
        public int Count { get; set; } = 0;
        public string? DisputeType { get; set; }
    }
}
