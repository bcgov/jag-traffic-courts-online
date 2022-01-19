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

namespace TrafficCourts.Citizen.Service.Configuration;

public static class ConfigurationExtensions
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

        Configure(builder, configuration?.RabbitMQ, logger);
        Configure(builder, configuration?.MassTransit, logger);
        Configure(builder, configuration?.FormRecognizer, logger);
        Configure(builder, configuration?.TicketSearchClient, logger);

        // add MediatR handlers in this program
        builder.Services.AddMediatR(typeof(ConfigurationExtensions).Assembly);
    }

    /// <summary>
    /// Gets a logger for application setup.
    /// </summary>
    /// <returns></returns>
    private static ILogger GetLogger()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();

        return logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="ConfigurationErrorsException">Configuration is not correct.</exception>
    private static void ValidateConfiguration(CitizenServiceConfiguration configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        Dictionary<string, List<string>> errors = new();

        // RabbitMQ
        if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Host)) AddError(errors, "RabbitMQ", "Host is not configured");
        if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Username)) AddError(errors, "RabbitMQ", "Username is not configured");
        if (string.IsNullOrEmpty(configuration?.RabbitMQ?.Password)) AddError(errors, "RabbitMQ", "Password is not configured");

        // MassTransit
        if (configuration?.MassTransit?.Transport is null) AddError(errors, "MassTransit", "Transport is not configured");

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
    private static void Configure(WebApplicationBuilder builder, MassTransitConfigurationProperties configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        builder.Services.AddMassTransit<CitizenServiceConfiguration>(builder);
    }

    /// <summary>
    /// Configures Form Recognizer.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    private static void Configure(WebApplicationBuilder builder, FormRecognizerConfigurationOptions configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);


    }

    /// <summary>
    /// Configures Ticket Search service.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    private static void Configure(WebApplicationBuilder builder, TicketSearchServiceConfigurationProperties configuration, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);


        builder.Services.AddSingleton(services =>
        {
            var address = configuration.Address;

            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
                ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } },
                ServiceProvider = services
            });

            return channel;
        });
    }
}