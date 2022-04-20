namespace TrafficCourts.Workflow.Service.Configuration
{
    /// <summary>
    /// Represents the smtp connection configuration
    /// </summary>
    public class SmtpConfiguration
    {
        /// <summary>
        /// The host of the smtp server
        /// </summary>
        public string? Host { get; set; }
        /// <summary>
        /// The port of the smtp server (it's usually 25)
        /// </summary>
        public int Port { get; set; }
    }
}
