using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TrafficCourts.Common.Configuration
{
    public static class SerilogExtensions
    {
        public static void UseSerilog<TConfiguration>(this WebApplicationBuilder builder)
             where TConfiguration : ISplunkConfiguration, new()
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Host.UseSerilog<TConfiguration>(builder.Configuration);
        }

        public static void UseSerilog<TConfiguration>(this IHostBuilder builder, ConfigurationManager configurationManager)
             where TConfiguration : ISplunkConfiguration, new()
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configurationManager == null) throw new ArgumentNullException(nameof(configurationManager));

            // map environment variables to configuration
            configurationManager.Add<SplunkConfigurationProvider>();

            builder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(configurationManager);

                // get the configuration type
                TConfiguration configuration = configurationManager.Get<TConfiguration>();

                var splunk = configuration.Splunk;
                if (splunk == null || string.IsNullOrEmpty(splunk.Url) || string.IsNullOrEmpty(splunk.Token))
                {
                    return;
                }

                HttpClientHandler? handler = null;

                if (!splunk.ValidatServerCertificate)
                {
                    handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }

                loggerConfiguration.WriteTo.EventCollector(
                    splunkHost: splunk.Url,
                    eventCollectorToken: splunk.Token,
                    sourceType: typeof(TConfiguration).Assembly?.GetName()?.Name,
                    //restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    messageHandler: handler);
            });
        }
    }
}
