using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NodaTime;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Citizen.Service.Logging;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.Configuration;

namespace TrafficCourts.Citizen.Service;

public static class Startup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
    /// <exception cref="ConfigurationErrorsException"></exception>
    public static void ConfigureApplication(this WebApplicationBuilder builder)
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

        var logger = GetLogger(builder);

        AddOpenTelemetry(builder, logger);

        ValidateConfiguration(configuration, logger); // throws ConfigurationErrorsException if configuration has issues

        if (configuration.TicketStorage == TicketStorageType.InMemory)
        {
            builder.AddInMemoryFilePersistence();
        } 
        else if (configuration.TicketStorage == TicketStorageType.ObjectStore)
        {
            builder.AddObjectStorageFilePersistence();
        }


        Configure(builder, configuration?.FormRecognizer, logger);
        Configure(builder, configuration?.TicketSearchClient, logger);

        builder.Services.AddMassTransit<CitizenServiceConfiguration>(builder);

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
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddSource(Diagnostics.Source.Name)
                .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddJaegerExporter();
        });
    }

    /// <summary>
    /// Gets a logger for application setup.
    /// </summary>
    /// <returns></returns>
    private static Serilog.ILogger GetLogger(WebApplicationBuilder builder)
    {
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
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <exception cref="ConfigurationErrorsException">Configuration is not correct.</exception>
    private static void ValidateConfiguration(CitizenServiceConfiguration configuration, Serilog.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        Dictionary<string, List<string>> errors = new();

        // MassTransit
        // RabbitMQ
        if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Host)) AddError(errors, "RabbitMQ", "Host is not configured");
        if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Username)) AddError(errors, "RabbitMQ", "Username is not configured");
        if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Password)) AddError(errors, "RabbitMQ", "Password is not configured");

        // FormRecognizer
        if (string.IsNullOrEmpty(configuration?.FormRecognizer?.ApiKey)) AddError(errors, "FormRecognizer", "ApiKey not specified");
        if (configuration?.FormRecognizer?.Endpoint is null) AddError(errors, "FormRecognizer", "Endpoint not specified");

        // TicketSearchClient
        if (string.IsNullOrEmpty(configuration?.TicketSearchClient?.Address)) AddError(errors, "TicketSearchClient", "Address not specified");

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                logger.Error("{Group} has configuration errors {Errors}", error.Key, error.Value);
            }

            Log.CloseAndFlush(); // force the writting of logs

            throw new ConfigurationErrorsException($"{errors.Count} configuration error(s), check previous log messages for details.");
        }
    }

    private static void AddError(Dictionary<string, List<string>> errors, string group, string error)
    {
        if (!errors.TryGetValue(group, out var items))
        {
            items = new List<string>();
            errors.Add(group, items);
        }

        items.Add(error);
    }

    /// <summary>
    /// Configures Form Recognizer.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, FormRecognizerConfigurationOptions? configuration, Serilog.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);

        if (configuration is null)
        {
            return;
        }

        builder.Services.AddSingleton<IFormRecognizerService>(service => {
            var logger = service.GetRequiredService<ILogger<FormRecognizerService>>();
            return new FormRecognizerService(configuration.ApiKey!, configuration.Endpoint!, logger);
        });

        builder.Services.AddSingleton<IFormRecognizerValidator, FormRecognizerValidator>();
    }

    /// <summary>
    /// Configures Ticket Search service.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, TicketSearchServiceConfigurationProperties? configuration, Serilog.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);

        if (configuration is null)
        {
            return;
        }

        string address = configuration.Address;

        if (string.IsNullOrEmpty(address))
        {
            throw new ConfigurationErrorsException("TicketSearchClient:Address is not configured");
        }

        ChannelCredentials credentials = configuration.Secure ? ChannelCredentials.SecureSsl : ChannelCredentials.Insecure;

        logger.Information("Configuring ticket search to use {Address} with {CredentialType}", address, configuration.Secure ? "secure" : "insecure");

        builder.Services.AddSingleton(services =>
        {
            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                Credentials = credentials,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } },
                ServiceProvider = services
            });

            return channel;
        });
    }
}
