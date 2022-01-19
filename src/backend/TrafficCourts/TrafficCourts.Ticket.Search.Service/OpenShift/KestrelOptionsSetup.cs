using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Options;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace TrafficCourts.Ticket.Search.Service.OpenShift
{
    internal class KestrelOptionsSetup : IConfigureOptions<KestrelServerOptions>
    {
        private readonly OpenShiftCertificateLoader _certificateLoader;
        private readonly IOptions<OpenShiftIntegrationOptions> _options;

        public KestrelOptionsSetup(IOptions<OpenShiftIntegrationOptions> options, OpenShiftCertificateLoader certificateLoader)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _certificateLoader = certificateLoader ?? throw new ArgumentNullException(nameof(certificateLoader));
        }

        public void Configure(KestrelServerOptions options)
        {
            if (_options.Value.UseHttps)
            {
                options.ListenAnyIP(8080, configureListen =>
                {
                    UseHttpsWithFullChain(configureListen);
                    // enable Http2, for gRPC
                    configureListen.Protocols = HttpProtocols.Http2;
                    configureListen.UseConnectionLogging();
                });
            }
            else
            {
                options.ListenAnyIP(8080, configureListen =>
                {
                    // enable Http2, for gRPC
                    configureListen.Protocols = HttpProtocols.Http2;
                    configureListen.UseConnectionLogging();
                });
            }

            // Also listen on port 8088 for health checks. Note that you won't be able to do gRPC calls on this port; 
            // it is only required because the OpenShift 3.11 health check system does not seem to be compatible with HTTP2.
            options.ListenAnyIP(8088, configureListen => { configureListen.Protocols = HttpProtocols.Http1; });
        }

        private ListenOptions UseHttpsWithFullChain(ListenOptions listenOptions)
        {
            ArgumentNullException.ThrowIfNull(listenOptions);

            var leafCertWithKey = _certificateLoader.GetCertificateWithKey();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // https://github.com/dotnet/runtime/issues/23749
                leafCertWithKey = new X509Certificate2(leafCertWithKey.Export(X509ContentType.Pkcs12));
            }

            // Import the full cert chain
            var fullChain = _certificateLoader.GetCertificateChain();
            var options = new SslServerAuthenticationOptions
            {
                // Don't go online to retrieve cert chain
                ServerCertificateContext = SslStreamCertificateContext.Create(leafCertWithKey, fullChain, offline: true)
            };

            return listenOptions.UseHttps(new TlsHandshakeCallbackOptions
            {
                OnConnection = context => new(options)
            });
        }
    }
}
