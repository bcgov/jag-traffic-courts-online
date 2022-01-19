namespace TrafficCourts.Ticket.Search.Service.OpenShift
{
    /// <summary>
    /// Contains the location where the TLS certificate should be stored.
    /// </summary>
    public class OpenShiftIntegrationOptions
    {
        public string CertificateMountPoint { get; set; }

        internal bool UseHttps => !string.IsNullOrEmpty(CertificateMountPoint);
    }
}
