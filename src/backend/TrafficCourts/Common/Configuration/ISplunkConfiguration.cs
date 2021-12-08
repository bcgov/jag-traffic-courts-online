namespace TrafficCourts.Common.Configuration
{
    public interface ISplunkConfiguration
    {
        SplunkConfigurationProperties? Splunk { get; set; }
    }
}
