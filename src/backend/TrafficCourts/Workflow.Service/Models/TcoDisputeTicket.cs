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
        public IList<DisputeDetail> DisputeDetails { get; set; } = new List<DisputeDetail>();

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
        public string Section { get; set; } = null!;
        public string Act { get; set; } = null!;
        public double Amount { get; set; } = Double.NaN;
    }

    public class DisputeDetail
    {
        public int Count { get; set; } = 0;
        public string? DisputeType { get; set; }
    }
}
