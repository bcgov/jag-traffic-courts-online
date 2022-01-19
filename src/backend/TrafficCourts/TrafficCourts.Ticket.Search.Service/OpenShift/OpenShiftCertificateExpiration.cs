using Microsoft.Extensions.Options;

namespace TrafficCourts.Ticket.Search.Service.OpenShift
{
    /// <summary>
    /// Handles stopping the application before the certificate expires.
    /// The hosting environment should automatically renew the certificate
    /// when half of its life has expired.
    /// </summary>
    internal class OpenShiftCertificateExpiration : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly OpenShiftCertificateLoader _certificateLoader;
        private readonly ILogger<OpenShiftCertificateExpiration> _logger;
        private readonly IOptions<OpenShiftIntegrationOptions> _options;

        public OpenShiftCertificateExpiration(
            IOptions<OpenShiftIntegrationOptions> options,
            OpenShiftCertificateLoader certificateLoader, 
            IHostApplicationLifetime applicationLifetime,
            ILogger<OpenShiftCertificateExpiration> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _certificateLoader = certificateLoader ?? throw new ArgumentNullException(nameof(certificateLoader));
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static TimeSpan RestartSpan => TimeSpan.FromMinutes(15);
        private static TimeSpan NotAfterMargin => TimeSpan.FromMinutes(15);

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            if (_options.Value.UseHttps)
            {
                _logger.LogInformation("Configured to use HTTPS");

                try
                {
                    var certificate = _certificateLoader.GetCertificateWithKey();
                    _logger.LogDebug("Certificate expires at {CertificateExpiration}", certificate.NotAfter.ToUniversalTime());

                    // will loop if the number of milliseconds to sleep is greater than int.MaxValue (24.8 days)
                    bool loop; 
                    {
                        loop = false;
                        var expiresAt = certificate.NotAfter - NotAfterMargin; // NotAfter is in local time.
                        var tillExpires = expiresAt - DateTime.Now;
                        if (tillExpires > TimeSpan.Zero)
                        {
                            if (tillExpires > RestartSpan)
                            {
                                // Wait until we are in the RestartSpan.
                                var delay = tillExpires - RestartSpan
                                            + TimeSpan.FromSeconds(new Random().Next((int)RestartSpan.TotalSeconds));
                                if (delay.TotalMilliseconds > int.MaxValue)
                                {
                                    // Task.Delay is limited to int.MaxValue.
                                    await Task.Delay(int.MaxValue, token);
                                    loop = true; // loop again
                                }
                                else
                                {
                                    await Task.Delay(delay, token);
                                }
                            }
                        }
                    }
                    while (loop) ;

                    // Our certificate expired, Stop the application.  OpenShift should regenerate the certificates automatically.
                    _logger.LogInformation("Certificate expires at {CertificateExpiration}. Stopping application.", certificate.NotAfter.ToUniversalTime());
                    _applicationLifetime.StopApplication();
                }
                catch (TaskCanceledException)
                {
                }
            }
            else
            {
                // shouldn't get here because this background should not be registered if not using HTTPS/OpenShift
                _logger.LogInformation("Not configured to use HTTPS");
            }
        }
    }

}
