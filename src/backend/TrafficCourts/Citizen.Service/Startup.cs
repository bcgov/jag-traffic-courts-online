using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NodaTime;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Configuration;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Citizen.Service.Logging;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging;
using ILogger = Serilog.ILogger;
using TrafficCourts.Citizen.Service.Services.Impl;
using StackExchange.Redis;

namespace TrafficCourts.Citizen.Service;

public static class Startup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
    /// <exception cref="ConfigurationErrorsException"></exception>
    public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // setup the mapping for friendly environment variables
        ((IConfigurationBuilder)builder.Configuration).Add(new EnvironmentVariablesConfigurationSource());

        if (builder.Environment.IsDevelopment())
        {
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        }

        builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
        });

        // configure application 
        var configuration = builder.Configuration.Get<CitizenServiceConfiguration>();

        builder.Services.UseConfigurationValidation();
        builder.UseTicketSearch(logger);

        AddOpenTelemetry(builder, logger);

        if (configuration.TicketStorage == TicketStorageType.InMemory)
        {
            builder.AddInMemoryFilePersistence();
        }
        else if (configuration.TicketStorage == TicketStorageType.ObjectStore)
        {
            builder.AddObjectStorageFilePersistence();
        }

        Configure(builder, configuration?.Redis, logger);

        // Form Recognizer
        builder.Services.ConfigureValidatableSetting<FormRecognizerOptions>(builder.Configuration.GetSection(FormRecognizerOptions.Section));
        builder.Services.AddTransient<IFormRecognizerService, FormRecognizerService>();
        builder.Services.AddTransient<IFormRecognizerValidator, FormRecognizerValidator>();

        // MassTransit
        builder.Services.AddMassTransit(builder.Configuration, logger);

        // add MediatR handlers in this program
        builder.Services.AddMediatR(typeof(Startup).Assembly);

        // use lowercase routes
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        builder.Services.AddTransient<IConfigureOptions<JsonOptions>, ConfigureJsonOptions>();

        builder.Services.AddSingleton<IClock>(SystemClock.Instance);

        builder.Services.AddRecyclableMemoryStreams();
    }

    private static void AddOpenTelemetry(WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        string? endpoint = builder.Configuration["OTEL_EXPORTER_JAEGER_ENDPOINT"];

        if (string.IsNullOrEmpty(endpoint))
        {
            logger.Information("Jaeger endpoint is not configured, no telemetry will be collected.");
            return;
        }

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(Diagnostics.ServiceName, serviceInstanceId: Environment.MachineName);

        builder.Services.AddOpenTelemetryTracing(options =>
        {
            options
                .SetResourceBuilder(resourceBuilder)
                .AddGrpcClientInstrumentation()
                .AddHttpClientInstrumentation(options =>
                {
                    // do not trace calls to splunk
                    options.Filter = (message) => message.RequestUri?.Host != "hec.monitoring.ag.gov.bc.ca";

                })
                .AddAspNetCoreInstrumentation()
                .AddSource(Diagnostics.Source.Name)
                .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddJaegerExporter();
        });
    }

    /// <summary>
    /// Configures Lookup Service and Redis Cache Service.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, RedisConfigurationProperties? configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);

        if (configuration is null)
        {
            configuration = new RedisConfigurationProperties();
        }
        ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configuration.ConnectionString);
        builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

        builder.Services.AddSingleton<ILookupService>(service =>
        {
            var redisConnection = service.GetRequiredService<IConnectionMultiplexer>();
            var logger = service.GetRequiredService<ILogger<RedisLookupService>>();
            return new RedisLookupService(redisConnection, logger);
        });

        builder.Services.AddSingleton<IRedisCacheService>(service =>
        {
            var redisConnection = service.GetRequiredService<IConnectionMultiplexer>();
            var logger = service.GetRequiredService<ILogger<RedisCacheService>>();
            return new RedisCacheService(redisConnection, logger);
        });
    }
}
