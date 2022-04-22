namespace TrafficCourts.Workflow.Service.Configuration
{
    /// <summary>
    /// Represents the email sending configuration
    /// </summary>
    public class EmailConfiguration
    {
        /// <summary>
        /// The default valid email address to send from if no from address specified in message.
        /// </summary>
        public string? Sender { get; set; }
        /// <summary>
        /// The optional list of email domains that messages can sent to. Values must have the format '@example.com'.
        /// </summary>
        public string[]? AllowList { get; set; }
    }
}
