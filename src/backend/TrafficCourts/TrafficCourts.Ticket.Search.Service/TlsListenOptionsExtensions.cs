using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TrafficCourts.Ticket.Search.Service
{
    /// <summary>
    /// https://github.com/dotnet/aspnetcore/issues/21513#issuecomment-914370034
    /// </summary>
    public static class TlsListenOptionsExtensions
    {
        public static ListenOptions UseHttpsWithFullChain(this ListenOptions listenOptions, string certPath, string keyPath)
        {
            ArgumentNullException.ThrowIfNull(certPath);
            ArgumentNullException.ThrowIfNull(keyPath);

            var leafCertWithKey = X509Certificate2.CreateFromPemFile(certPath, keyPath);

            // Import the full cert chain
            var fullChain = new X509Certificate2Collection();
            fullChain.ImportFromPemFile(certPath);

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

