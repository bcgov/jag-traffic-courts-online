using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Workflow.Service.Configuration
{
    /// <summary>
    /// Represents the smtp connection configuration
    /// </summary>
    public class SmtpConfiguration : IValidatable
    {
        public const string Section = "SmtpConfiguration";

        /// <summary>
        /// The host of the smtp server
        /// </summary>
        public string? Host { get; set; }
        /// <summary>
        /// The port of the smtp server (it's usually 25)
        /// </summary>
        public int Port { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Host)) throw new SettingsValidationException(Section, nameof(Host), "is required");
        }
    }
}
