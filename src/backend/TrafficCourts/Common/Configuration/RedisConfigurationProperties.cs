using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Configuration;

[ExcludeFromCodeCoverage]
public class RedisConfigurationProperties
{
    public string ConnectionString { get; set; } = "localhost:6379";
}
