using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Arc.Dispute.Service.Configuration
{
    /// <summary>
    /// Represents the connection SFTP options.
    /// </summary>
    public class SftpConnectionOptions : IValidatable
    {
        public const string Section = "Sftp";
        private const ushort DefaultPort = 22;
        private ushort _port;

        public string? Host { get; set; }
        
        /// <summary>
        /// Gets the port or the default port (22) if not set.
        /// </summary>
        public ushort Port 
        { 
            get 
            {
                return (_port <= 0) ? DefaultPort : _port;
            }
            
            set => _port = value; 
        }

        public string? Username { get; set; }

        /// <summary>
        /// The SFTP password.  The <see cref="SshPrivateKeyPath"/> will be used if exists.
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// The SSH private key. If the private key exists, it will be used instead of <see cref="Password"/>.
        /// </summary>
        public string? SshPrivateKeyPath { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Host)) throw new SettingsValidationException(Section, nameof(Host), "is required");
            if (string.IsNullOrEmpty(Username)) throw new SettingsValidationException(Section, nameof(Username), "is required");

            if (!string.IsNullOrEmpty(SshPrivateKeyPath))
            {
                if (!File.Exists(SshPrivateKeyPath))
                {
                    throw new SettingsValidationException(Section, nameof(SshPrivateKeyPath), "file does not exist");
                }
            }
            else if (string.IsNullOrEmpty(Password))
            {
                throw new SettingsValidationException(Section, nameof(Password), "is required when no private key has been specified");
            }
        }
    }
}
