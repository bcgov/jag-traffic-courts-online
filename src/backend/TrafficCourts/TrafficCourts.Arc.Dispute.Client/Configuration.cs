using Microsoft.Extensions.Configuration;

namespace TrafficCourts.Arc.Dispute.Client
{
    internal class Configuration
    {
        public string Scheme { get; set; } = "http";
        public string? Host { get; set; }
        public int Port { get; set; }
        public Uri Uri => new Uri($"{Scheme}://{Host}:{Port}");
    }
}
