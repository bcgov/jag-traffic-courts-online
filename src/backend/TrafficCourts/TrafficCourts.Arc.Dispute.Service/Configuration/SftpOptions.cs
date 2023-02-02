using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Arc.Dispute.Service.Configuration
{

    /// <summary>
    /// Represents the non-connection SFTP options.
    /// </summary>
    public class SftpOptions : IValidatable
    {
        public const string Section = "Sftp";

        public string RemotePath { get; set; } = "tco-submissions";

        public void Validate()
        {
            if (string.IsNullOrEmpty(RemotePath)) throw new SettingsValidationException(Section, nameof(RemotePath), "is required");
            if (RemotePath.Any(c => Char.IsWhiteSpace(c))) throw new SettingsValidationException(Section, nameof(RemotePath), "must not contain whitespace");
        }
    }
}
