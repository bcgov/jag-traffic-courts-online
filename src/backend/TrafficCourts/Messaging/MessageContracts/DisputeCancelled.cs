namespace TrafficCourts.Messaging.MessageContracts
{
    public class DisputeCancelled
    {
        public long Id { get; set; } = -1;
        public string? Email { get; set; }
    }
}
