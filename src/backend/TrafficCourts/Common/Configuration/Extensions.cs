using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using StackExchange.Redis;
using System.Diagnostics;

namespace TrafficCourts.Common.Configuration;

public static class Extensions
{
    /// <summary>
    /// Adds ini formatted Vault Secrets to the configuration
    /// </summary>
    /// <param name="configurationManager"></param>
    public static void AddVaultSecrets(this ConfigurationManager configurationManager, Serilog.ILogger logger)
    {
        // standard directory Vault stores secrets
        const string vaultSecrets = "/vault/secrets";

        if (!Directory.Exists(vaultSecrets))
        {
            logger.Information("Vault {Directory} does not exist, will not load Vault secrets", vaultSecrets);
            return;
        }

        foreach (var file in Directory.EnumerateFiles(vaultSecrets, "*.ini", SearchOption.TopDirectoryOnly))
        {
            logger.Debug("Loading secrets from {File}", file);
            configurationManager.AddIniFile(file, optional: false, reloadOnChange: false); // assume we can read
        }
    }

    /// <summary>
    /// Adds Redis and provides <see cref="IConnectionMultiplexer"/> in dependency injection.
    /// </summary>
    /// <param name="builder">The builder for the application.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
    public static void AddRedis(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.ConfigureValidatableSetting<RedisOptions>(builder.Configuration.GetSection(RedisOptions.Section));

        builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<RedisOptions>();
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configuration.ConnectionString);
            return connectionMultiplexer;
        });
    }

    /// <summary>
    /// Gets top level program logger.
    /// </summary>
    /// <param name="builder">The builder for the application.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
    /// <returns>A <see cref="Serilog.ILogger"/> instance that will log to console.</returns>
    public static ILogger GetProgramLogger(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var configuration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console();

        if (builder.Environment.IsDevelopment())
        {
            configuration.WriteTo.Debug();
        }

        return configuration.CreateLogger();
    }

    /// <summary>
    /// Configures the host to use Serilog logging in a standard way using the default exception destructurers. 
    /// It configures logging from configuration.
    /// </summary>
    /// <param name="builder">The builder for applications.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        AddSerilog(builder, Array.Empty<IExceptionDestructurer>());
    }

    /// <summary>
    /// Configures the host to use Serilog logging in a standard way. It configures logging from configuration.
    /// </summary>
    /// <param name="builder">The builder for the application</param>
    /// <param name="additionalDestructurers">Additioanl <see cref="IExceptionDestructurer"/> to destructure exceptions.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="additionalDestructurers"/> is null.</exception>
    public static void AddSerilog(this WebApplicationBuilder builder, IEnumerable<IExceptionDestructurer> additionalDestructurers)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(additionalDestructurers);

        // configure Serilog in a standard way
        builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var destructuringOptionsBuilder = new DestructuringOptionsBuilder();
            var destructurers = destructuringOptionsBuilder
                .WithDefaultDestructurers()
                .WithDestructurers(additionalDestructurers);
           
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.WithExceptionDetails(destructurers);
        });
    }

    /// <summary>
    /// Adds OpenTelemetry configuration.
    /// </summary>
    /// <param name="builder">The builder for the application</param>
    /// <param name="activitySource">The application <see cref="ActivitySource"/> to monitor.</param>
    /// <param name="logger">The program logger to log </param>
    /// <param name="configure">Callback action to configure the OpenTelemetry.Trace.TracerProviderBuilder.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/>, <paramref name="activitySource"/> or <param name="logger"/> is null.
    /// </exception>
    public static void AddOpenTelemetry(
        this WebApplicationBuilder builder, 
        ActivitySource activitySource, 
        Serilog.ILogger logger, 
        Action<TracerProviderBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(activitySource);
        ArgumentNullException.ThrowIfNull(logger);

        string? endpoint = builder.Configuration["OTEL_EXPORTER_JAEGER_ENDPOINT"];

        if (string.IsNullOrEmpty(endpoint))
        {
            logger.Information("Jaeger endpoint is not configured, no telemetry will be collected.");
            return;
        }

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(activitySource.Name, serviceInstanceId: Environment.MachineName);

        builder.Services.AddOpenTelemetryTracing(options =>
        {
            options
                .SetResourceBuilder(resourceBuilder)
                .AddHttpClientInstrumentation(options =>
                {
                    // do not trace calls to splunk
                    options.Filter = (message) => message.RequestUri?.Host != "hec.monitoring.ag.gov.bc.ca";
                })
                .AddAspNetCoreInstrumentation()
                .AddSource(activitySource.Name)
                .AddJaegerExporter();

            if (configure is not null)
            {
                configure(options);
            }            
        });
    }
}
