using Microsoft.IO;
using BCGov.VirusScan.Api.Network;
using BCGov.VirusScan.Api.Services;
using OpenTelemetry;
using BCGov.VirusScan.Api.Monitoring;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddVirusScan(this IServiceCollection services)
    {
        services.AddSingleton<RecyclableMemoryStreamManager>();
        services.AddTransient<IRecyclableMemoryStreamManager, DefaultRecyclableMemoryStreamManager>();

        services.AddTransient<ITcpClient, TcpClientWrapper>();

        services.AddTransient<IVirusScanService, VirusScanService>();
        return services;
    }

    /// <summary>
    /// Adds OpenTelemetry configuration to the web application.
    /// </summary>
    /// <param name="builder"></param>
    public static void AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOpenTelemetry()
            //.WithTracing(builder => builder.AddSource(Instrumentation.ActivitySource))
            .WithMetrics(builder =>
            {
                builder
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation();

                builder.AddMeter(Instrumentation.MeterName);

                builder.AddPrometheusExporter();
            });
    }
}
