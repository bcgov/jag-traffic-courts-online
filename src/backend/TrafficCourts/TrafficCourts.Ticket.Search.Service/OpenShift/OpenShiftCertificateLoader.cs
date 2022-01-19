using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace TrafficCourts.Ticket.Search.Service.OpenShift
{
    /// <summary>
    /// Reads the certificate from the certificate mount point.
    /// </summary>
    internal class OpenShiftCertificateLoader
    {
        private readonly IOptions<OpenShiftIntegrationOptions> _options;

        public OpenShiftCertificateLoader(IOptions<OpenShiftIntegrationOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public X509Certificate2 GetCertificateWithKey()
        {
            var certificateMountPoint = _options.Value.CertificateMountPoint;
            var certificateFile = Path.Combine(certificateMountPoint, "tls.crt");
            var keyFile = Path.Combine(certificateMountPoint, "tls.key");
            var certificate = X509Certificate2.CreateFromPemFile(certificateFile, keyFile);

            return certificate;
        }

        public X509Certificate2Collection GetCertificateChain()
        {
            var certificateMountPoint = _options.Value.CertificateMountPoint;
            var fullChain = new X509Certificate2Collection();
            fullChain.ImportFromPemFile(Path.Combine(certificateMountPoint, "tls.crt"));
            return fullChain;
        }
    }

}
