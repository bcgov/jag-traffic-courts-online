namespace TrafficCourts.Common.Configuration
{
    public class SplunkConfigurationProvider : TrafficCourtsConfigurationProvider
    {
        public override void Load()
        {
            Add("SPLUNK_URL", $"{nameof(ISplunkConfiguration.Splunk)}:{nameof(SplunkConfigurationProperties.Url)}");
            Add("SPLUNK_TOKEN", $"{nameof(ISplunkConfiguration.Splunk)}:{nameof(SplunkConfigurationProperties.Token)}");
        }
    }
}

