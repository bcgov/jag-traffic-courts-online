using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using System.Configuration;
using Serilog;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.Configuration;
using MediatR;
using TrafficCourts.Common.Configuration;
using ILogger = Serilog.ILogger;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Services.Impl;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Citizen.Service.Validators;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

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

        builder.UseSerilog<CitizenServiceConfiguration>(); // configure logging

        // configure application
        var configuration = builder.Configuration.Get<CitizenServiceConfiguration>();

        var logger = GetLogger();

        ValidateConfiguration(configuration, logger); // throws ConfigurationErrorsException if configuration has issues

        if (configuration.RabbitMQ is not null)
        {
            Configure(builder, configuration.RabbitMQ, logger);
        }

        Configure(builder, configuration?.FlatFileLookupService, logger);
        Configure(builder, configuration?.MassTransit, logger);
        Configure(builder, configuration?.FormRecognizer, logger);
        Configure(builder, configuration?.TicketSearchClient, logger);

        // add MediatR handlers in this program
        builder.Services.AddMediatR(typeof(Startup).Assembly);

        // use lowercase routes
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        builder.Services.AddTransient<IConfigureOptions<JsonOptions>, ConfigureJsonOptions>();
    }

    /// <summary>
    /// Gets a logger for application setup.
    /// </summary>
    /// <returns></returns>
    private static ILogger GetLogger()
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();

        return logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <exception cref="ConfigurationErrorsException">Configuration is not correct.</exception>
    private static void ValidateConfiguration(CitizenServiceConfiguration configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        Dictionary<string, List<string>> errors = new();

        // MassTransit
        if (configuration?.MassTransit?.Transport is null)
        {
            AddError(errors, "MassTransit", "Transport is not configured");
        }
        else
        {
            var transport = configuration.MassTransit.Transport;
            if (transport == MassTransitTransport.RabbitMQ)
            {
                // RabbitMQ
                if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Host)) AddError(errors, "RabbitMQ", "Host is not configured");
                if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Username)) AddError(errors, "RabbitMQ", "Username is not configured");
                if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Password)) AddError(errors, "RabbitMQ", "Password is not configured");
            }
        }

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

            throw new ConfigurationErrorsException($"{errors.Count} configuration errors, check previous log messages for details.");
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
    /// Configures RabbitMQ
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, RabbitMQConfigurationProperties configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);
    }

    /// <summary>
    /// Configures MassTransit.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, MassTransitConfigurationProperties? configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);

        if (configuration is null)
        {
            return;
        }

        builder.Services.AddMassTransit<CitizenServiceConfiguration>(builder);
    }

    /// <summary>
    /// Configures Form Recognizer.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, FormRecognizerConfigurationOptions? configuration, ILogger logger)
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

        builder.Services.AddSingleton<IFormRecognizerValidator>(service => {
            var lookupService = service.GetRequiredService<ILookupService>();
            return new FormRecognizerValidator(lookupService);
        });
    }

    /// <summary>
    /// Configures Lookup Service.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, FlatFileLookupServiceConfiguration? configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(logger);

        builder.Services.AddSingleton<ILookupService>(service => {
            var logger = service.GetRequiredService<ILogger<FlatFileLookupService>>();
            if (configuration is null) {
                configuration = new FlatFileLookupServiceConfiguration();
            }
            return new FlatFileLookupService(configuration.BasePath!, logger);
        });
    }

    /// <summary>
    /// Configures Ticket Search service.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    private static void Configure(WebApplicationBuilder builder, TicketSearchServiceConfigurationProperties? configuration, ILogger logger)
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
