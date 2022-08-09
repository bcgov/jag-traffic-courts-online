namespace TrafficCourts.Arc.Dispute.Client
{
    internal class Configuration
    {
        public string Scheme { get; set; } = "http";
        public string? Host { get; set; }
        public int Port { get; set; }
    }
}
