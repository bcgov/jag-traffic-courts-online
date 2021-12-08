namespace TrafficCourts.Common.Configuration
{
    public class SplunkConfigurationProperties
    {
        /// <summary>
        /// HTTP Event Collector (HEC) Url
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// HTTP Event Collector (HEC) Token
        /// </summary>
        public string? Token { get; set; }

        public bool ValidatServerCertificate { get; set; } = true;
    }
}
