using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using StackExchange.Redis;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Configuration;

[ExcludeFromCodeCoverage]
public static class Extensions
{
    /// <summary>
    /// Adds Redis and provides <see cref="IConnectionMultiplexer"/> in dependency injection.
    /// </summary>
    /// <param name="builder">The builder for the application.</param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
    public static void AddRedis(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // keep in case someone is using these options
        builder.Services.ConfigureValidatableSetting<RedisOptions>(builder.Configuration.GetSection(RedisOptions.Section));

        // create a single ConnectionMultiplexer and register the instance and return that for Redis based IDistributedCache 
        var connectionString = GetRedisConnectionString(builder.Configuration).ConnectionString;
        if (!string.IsNullOrEmpty(connectionString))
        {
            // TODO: defer connecting to Redis until we need the connection, otherwise failing to connect at start up will cause the service to fail to start
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);

            builder.Services.AddSingleton(connectionMultiplexer);

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
            });
        }
        else
        {
            throw new SettingsValidationException(nameof(RedisOptions), nameof(RedisOptions.ConnectionString), "is required");
        }
    }

    private static RedisOptions GetRedisConnectionString(IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(RedisOptions.Section);
        RedisOptions options = new();
        section.Bind(options);
        return options;
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
        ILogger logger, 
        Action<TracerProviderBuilder>? configure = null,
        params string[] meters)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(activitySource);
        ArgumentNullException.ThrowIfNull(logger);

        AddTracing(builder, activitySource, logger, configure);
        AddMetrics(builder.Services, meters);
    }

    private static void AddTracing(WebApplicationBuilder builder, ActivitySource activitySource, ILogger logger, Action<TracerProviderBuilder>? configure = null)
    {
        string? endpoint = builder.Configuration["OTEL_EXPORTER_JAEGER_ENDPOINT"];

        if (string.IsNullOrEmpty(endpoint))
        {
            logger.Information("Jaeger endpoint is not configured, no telemetry traces will be collected.");
            return;
        }

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(activitySource.Name, serviceInstanceId: Environment.MachineName);

        builder.Services.AddOpenTelemetry().WithTracing(builder => 
        {
            builder
                .SetResourceBuilder(resourceBuilder)
                .AddHttpClientInstrumentation(builder =>
                {
                    builder.FilterHttpRequestMessage = HttpClientRequestFilter;
                })
                .AddAspNetCoreInstrumentation(options => options.Filter = AspNetCoreRequestFilter)
                .AddSource(activitySource.Name)
                .AddJaegerExporter();

            if (configure is not null)
            {
                configure(builder);
            }
        });
    }

    private static bool AspNetCoreRequestFilter(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        // do not trace metrics calls to GET /metrics
       return !(httpContext.Request.Method == "GET" && httpContext.Request.Path == "/metrics");
    }

    private static bool HttpClientRequestFilter(HttpRequestMessage message)
    {
        // do not trace calls to splunk
        return message.RequestUri?.Host != "hec.monitoring.ag.gov.bc.ca";
    }

    private static void AddMetrics(IServiceCollection services, params string[] meters)
    {
        services.AddOpenTelemetry().WithMetrics(builder => 
        {
            builder
                .AddProcessInstrumentation()
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            if (meters.Length != 0)
            {
                builder.AddMeter(meters);
            }

            builder.AddPrometheusExporter();
        });
    }
}
