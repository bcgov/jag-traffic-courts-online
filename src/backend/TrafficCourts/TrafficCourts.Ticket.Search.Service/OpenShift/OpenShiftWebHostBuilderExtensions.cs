using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using TrafficCourts.Ticket.Search.Service.OpenShift;

namespace Microsoft.AspNetCore.Hosting;

public static class OpenShiftWebHostBuilderExtensions
{
    public static WebApplicationBuilder UseOpenShiftIntegration(this WebApplicationBuilder builder, Action<OpenShiftIntegrationOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configureOptions);

        if (PlatformEnvironment.IsOpenShift)
        {
            builder.WebHost.UseUrls();

            builder.WebHost.ConfigureServices(services =>
            {
                services.Configure(configureOptions);
                services.AddSingleton<OpenShiftCertificateLoader>();
                services.AddSingleton<IConfigureOptions<KestrelServerOptions>, KestrelOptionsSetup>();
                services.AddSingleton<IHostedService, OpenShiftCertificateExpiration>();
            });

        }

        return builder;
    }
}
