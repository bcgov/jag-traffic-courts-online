namespace TrafficCourts.Messaging.MessageContracts
{
    public class DisputeRejected
    {
        public string Reason { get; set; } = String.Empty;
        public string? Email { get; set; }
        public string? TicketNumber { get; set; } 
        public Guid NoticeOfDisputeGuid { get; set; }
    }
}
