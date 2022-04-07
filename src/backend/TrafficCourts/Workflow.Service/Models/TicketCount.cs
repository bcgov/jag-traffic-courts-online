
namespace TrafficCourts.Workflow.Service.Models
{
    public class TicketCount
    {
        public string OffenceDeclaration { get; set; } = null!;
        public bool TimeToPayRequest { get; set; }
        public bool FineReductionRequest { get; set; }
    }
}
