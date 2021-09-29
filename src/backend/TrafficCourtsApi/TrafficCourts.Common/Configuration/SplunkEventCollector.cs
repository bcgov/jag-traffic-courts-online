using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TrafficCourts.Common.Configuration
{
    /// <summary>
    /// Helper class to configure the Splunk Event Collector in a standard way. 
    /// </summary>
    public static class SplunkEventCollector
    {
        /// <summary>
        /// Configures Serilog to use HTTP Event Collector if the Splunk Url and HEC Token are available in the configuration.
        /// </summary>
        /// <param name="hostingContext">The hosting context.</param>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        public static void Configure(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
        {
            if (hostingContext == null) throw new ArgumentNullException(nameof(hostingContext));
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));

            IConfiguration configuration = hostingContext.Configuration;
            loggerConfiguration.ReadFrom.Configuration(configuration);

            // because splunk may not be configured with a valid TLS certificate, we need to ignore certificate errors
            // otherwise, the event collector could be configured using the configuration system
            var splunkUrl = configuration["Splunk:Url"];
            var splunkToken = configuration["Splunk:Token"];
            var sourceType = configuration["SOURCE_TYPE"];

            if (string.IsNullOrWhiteSpace(splunkToken) || string.IsNullOrWhiteSpace(splunkUrl))
            {
                Log.Warning("Splunk logging sink is not configured properly, check Splunk:Url and Splunk:Token configuration settings. Equivalent variables: SPLUNK__URL and SPLUNK__TOKEN.");
            }
            else
            {
                loggerConfiguration
                    .WriteTo.EventCollector(
                        splunkHost: splunkUrl,
                        eventCollectorToken: splunkToken,
                        sourceType: sourceType,
#pragma warning disable CA2000 // Dispose objects before losing scope
                        messageHandler: new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation
                        });
#pragma warning restore CA2000 // Dispose objects before losing scope

            }
        }

        internal static bool ServerCertificateCustomValidation(HttpRequestMessage message, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
