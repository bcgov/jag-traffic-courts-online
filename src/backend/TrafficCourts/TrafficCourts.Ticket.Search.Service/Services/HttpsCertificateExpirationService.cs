using System.Security.Cryptography.X509Certificates;

namespace TrafficCourts.Ticket.Search.Service.Services
{
    public interface IServiceCertificateProvider
    {
        X509Certificate2 ServiceCertificate { get; }
    }

    //internal class OpenShiftServiceCertificateProvider : IServiceCertificateProvider
    //{
    //    private X509Certificate2 _certificate;

    //    public X509Certificate2 ServiceCertificate => throw new NotImplementedException();

    //    private X509Certificate2 GetCertificate()
    //    {
    //        if (_certificate is null)
    //        {
    //            var certificateMountPoint = _options.Value.CertificateMountPoint;
    //            var certificateFile = Path.Combine(certificateMountPoint, "tls.crt");
    //            var keyFile = Path.Combine(certificateMountPoint, "tls.key");
    //            _certificate = X509Certificate2.CreateFromPemFile(certificateFile, keyFile);
    //        }

    //        return _certificate;
    //    }
    //}

    //internal class HttpsCertificateExpirationService : BackgroundService
    //{
    //    private readonly IHostApplicationLifetime _applicationLifetime;
    //    private readonly OpenShiftCertificateLoader _certificateLoader;
    //    private readonly ILogger<HttpsCertificateExpirationService> _logger;
    //    private readonly IOptions<OpenShiftIntegrationOptions> _options;

    //    public HttpsCertificateExpirationService(
    //        IOptions<OpenShiftIntegrationOptions> options,
    //        OpenShiftCertificateLoader certificateLoader, IHostApplicationLifetime applicationLifetime,
    //        ILogger<HttpsCertificateExpirationService> logger)
    //    {
    //        _options = options;
    //        _certificateLoader = certificateLoader;
    //        _applicationLifetime = applicationLifetime;
    //        _logger = logger;
    //    }

    //    private static TimeSpan RestartSpan => TimeSpan.FromMinutes(15);
    //    private static TimeSpan NotAfterMargin => TimeSpan.FromMinutes(15);

    //    protected override async Task ExecuteAsync(CancellationToken token)
    //    {
    //        if (_options.Value.UseHttps)
    //        {
    //            try
    //            {
    //                var certificate = _certificateLoader.ServiceCertificate;
    //                bool loop;
    //                {
    //                    loop = false;
    //                    var expiresAt = certificate.NotAfter - NotAfterMargin; // NotAfter is in local time.
    //                    var now = DateTime.Now;
    //                    var tillExpires = expiresAt - now;
    //                    if (tillExpires > TimeSpan.Zero)
    //                        if (tillExpires > RestartSpan)
    //                        {
    //                            // Wait until we are in the RestartSpan.
    //                            var delay = tillExpires - RestartSpan
    //                                        + TimeSpan.FromSeconds(new Random().Next((int)RestartSpan.TotalSeconds));
    //                            if (delay.TotalMilliseconds > int.MaxValue)
    //                            {
    //                                // Task.Delay is limited to int.MaxValue.
    //                                await Task.Delay(int.MaxValue, token);
    //                                loop = true;
    //                            }
    //                            else
    //                            {
    //                                await Task.Delay(delay, token);
    //                            }
    //                        }
    //                }
    //                while (loop) ;
    //                // Our certificate expired, Stop the application.  OpenShift should regenerate the certificates automatically.
    //                _logger.LogInformation("Certificate expires at {CertificateExpiration}. Stopping application.", certificate.NotAfter.ToUniversalTime());
    //                _applicationLifetime.StopApplication();
    //            }
    //            catch (TaskCanceledException)
    //            {
    //            }
    //        }
    //        else
    //        {
    //            // shouldn't get here because this background should not be registered if not using HTTPS/OpenShift
    //            _logger.LogInformation("Not configured to use HTTPS");
    //        }
    //    }
    //}
}
