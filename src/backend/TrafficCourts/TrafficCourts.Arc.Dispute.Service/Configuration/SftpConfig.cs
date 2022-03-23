namespace TrafficCourts.Arc.Dispute.Service.Configuration
{
    public class SftpConfig
    {
        public const string Section = "Sftp";
        private const int DefaultPort = 22;
        private int _port;

        public string? Host { get; set; }
        
        /// <summary>
        /// Gets the port or the default port (22) if not set.
        /// </summary>
        public int Port 
        { 
            get 
            {
                return (_port <= 0) ? DefaultPort : _port;
            }
            
            set => _port = value; 
        }

        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? SshPrivateKeyPath { get; set; }
    }
}
