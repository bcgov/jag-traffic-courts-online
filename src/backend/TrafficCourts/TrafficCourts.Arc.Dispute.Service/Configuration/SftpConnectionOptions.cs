using Renci.SshNet;
using Renci.SshNet.Common;
using System.Text;
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

        public string SshPrivateKey { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Host)) throw new SettingsValidationException(Section, nameof(Host), "is required");
            if (string.IsNullOrEmpty(Username)) throw new SettingsValidationException(Section, nameof(Username), "is required");

            // validate credentials
            try
            {
                var privateKey = GetPrivateKey();
                if (privateKey is not null)
                {
                    return; // good!
                }
            }
            catch (SshException)
            {
                if (!string.IsNullOrEmpty(SshPrivateKey))
                {
                    throw new SettingsValidationException(Section, nameof(SshPrivateKey), "contains an invalid private key file");
                }

                throw new SettingsValidationException(Section, nameof(SshPrivateKeyPath), "contains an invalid private key file");
            }

            if (string.IsNullOrEmpty(Password))
            {
                throw new SettingsValidationException(Section, nameof(Password), "is required when no private key has been specified");
            }
        }

        /// <summary>
        /// Loads the private key based on the configuration.
        /// </summary>
        /// <returns>The <see cref="PrivateKeyFile"/> or null of the private key is not available.</returns>
        /// <exception cref="SshException">The private key is not well formed</exception>
        public PrivateKeyFile? GetPrivateKey()
        {
            if (!string.IsNullOrEmpty(SshPrivateKey))
            {
                var bytes = Encoding.ASCII.GetBytes(SshPrivateKey);
                MemoryStream stream = new MemoryStream(bytes);

                PrivateKeyFile privateKey = new(stream); // throws SshException if the private key is not well formed
                return privateKey;
            }

            if (!string.IsNullOrEmpty(SshPrivateKeyPath) && File.Exists(SshPrivateKeyPath))
            {
                PrivateKeyFile privateKey = new(SshPrivateKeyPath); // throws SshException if the private key is not well formed
                return privateKey;
            }

            return null; // private key not available
        }
    }
}
