using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class RedisConfigurationProperties
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Password { get; set; }
    }
}

