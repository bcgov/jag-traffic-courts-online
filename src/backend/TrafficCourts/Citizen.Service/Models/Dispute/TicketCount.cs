namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class TicketCount
    {
        public string OffenceDeclaration { get; set; } = String.Empty;
        public bool TimeToPayRequest { get; set; }
        public bool FineReductionRequest { get; set; }
    }
}
