namespace TrafficCourts.Arc.Dispute.Service.Configuration
{
    public class SftpConfig
    {
        public const string Section = "Sftp";
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
