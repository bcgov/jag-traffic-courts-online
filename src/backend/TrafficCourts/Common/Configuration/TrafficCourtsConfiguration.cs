using Serilog.Events;

namespace TrafficCourts.Common.Configuration;

public abstract class TrafficCourtsConfiguration : ISplunkConfiguration
{
    public SplunkConfigurationProperties? Splunk { get; set; }
}
