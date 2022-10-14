using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Reflection;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;
using TrafficCourts.Messaging.Configuration;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Messaging;

public static class BusConfiguratorExtensions
{
    public const string MassTransitSection = "MassTransit";

    /// <summary>
    /// Adds MassTransit and its dependencies to the collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="configureBusRegistration">optional bus registration configration function</param>
    /// <param name="configureBusFactory">ptional bus registration configration function</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ConfigurationErrorsException"></exception>
    public static void AddMassTransit(this IServiceCollection services,
        string serviceName,
        IConfiguration configuration, 
        Serilog.ILogger logger, 
        Action<IBusRegistrationConfigurator>? configureBusRegistration = null,
        Action<IBusFactoryConfigurator>? configureBusFactory = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceName);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);

        // determine the transport
        var transport = configuration.GetSection($"{MassTransitSection}:Transport").Get<MassTransitTransport>();

        
        if (transport == MassTransitTransport.RabbitMq)
        {
            logger.Information("Using MassTransit Transport: {Transport}", transport);
            services.AddMassTransit(config => UseRabbitMq(config, services, serviceName, configuration, configureBusRegistration, configureBusFactory));
        }
        else if (transport == MassTransitTransport.InMemory)
        {
            logger.Information("Using MassTransit Transport: {Transport}", transport);
            services.AddMassTransit(config => UseInMemory(config, configuration, configureBusRegistration));
        }
        else
        {
            string message = $"Unknown MassTransit Transport: {transport}. Value values are {nameof(MassTransitTransport.RabbitMq)} and {nameof(MassTransitTransport.InMemory)}";

#pragma warning disable Serilog004 // Constant MessageTemplate verifier - fatal error that will terminate the process
            logger.Fatal(message);
#pragma warning restore Serilog004 // Constant MessageTemplate verifier

            throw new ConfigurationErrorsException(message);
        }
    }

    private static void UseInMemory(IBusRegistrationConfigurator config, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configureBusRegistration)
    {
        configureBusRegistration?.Invoke(config); // add consumers, etc
        config.SetKebabCaseEndpointNameFormatter();
        config.UsingInMemory();
    }

    private static void UseRabbitMq(
        IBusRegistrationConfigurator config, 
        IServiceCollection services, 
        string serviceName,
        IConfiguration configuration, 
        Action<IBusRegistrationConfigurator>? configureBusRegistration,
        Action<IBusFactoryConfigurator>? configureBusFactory)
    {
        services.ConfigureValidatableSetting<RabbitMqHostOptions>(configuration.GetSection(RabbitMqHostOptions.Section));
        
        configureBusRegistration?.Invoke(config); // add consumers, etc

        config.SetKebabCaseEndpointNameFormatter();

        config.UsingRabbitMq((context, configure) => 
        {
            var options = context.GetRequiredService<RabbitMqHostOptions>();
            string connectionName = GetConnectionName(options);

            configure.MessageTopology.SetEntityNameFormatter(new PrefixEntityNameFormatter("Messages"));
            configure.SendTopology.ErrorQueueNameFormatter = new ErrorQueueNameFormatter();
            configure.SendTopology.DeadLetterQueueNameFormatter = new DeadLetterQueueNameFormatter();

            // enable instrumentation using the built-in .NET Meter class, which can be collected by OpenTelemetry
            configure.UseInstrumentation(serviceName: serviceName);

            configure.Host(options.Host, options.Port, options.VirtualHost, connectionName, host =>
            {
                host.Username(options.Username);
                host.Password(options.Password);
            });

            configure.UseConcurrencyLimit(options.Retry.ConcurrencyLimit);
            
            // sets the global message try policy
            configure.UseMessageRetry(r =>
            {
                r.Ignore<ArgumentNullException>();
                r.Ignore<InvalidOperationException>();

                // other retry options are:
                //   Exponential - int retryLimit, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta
                //   Incremental - int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement
                //   Interval    - int retryCount, TimeSpan interval
                //   Intervals   - TimeSpan[] intervals
                //   Intervals   - int[] intervals
                //   Immediate   - int retryLimit
                //   None        -
                r.Interval(options.Retry.Times, TimeSpan.FromMinutes(options.Retry.Interval));
            });

            configure.ConfigureEndpoints(context);

            configure.ConfigureJsonSerializerOptions(settings =>
            {
                settings.Converters.Add(new DateOnlyJsonConverter());
                settings.Converters.Add(new TimeOnlyJsonConverter());
                settings.Converters.Add(new JsonStringEnumConverter());
                return settings;
            });

            // should we call the user's configuration at the start or end?
            configureBusFactory?.Invoke(configure);
        });
    }



    /// <summary>
    /// Gets the connection name that will be displayed in the RabbitMq management console
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private static string GetConnectionName(RabbitMqHostOptions options)
    {
        string? connectionName = options.ClientProvidedName;
        if (string.IsNullOrWhiteSpace(connectionName))
        {
            connectionName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "Unknown";
        }

        connectionName += " (" + Environment.MachineName + ")";
        return connectionName;
    }
}

public class DeadLetterQueueNameFormatter : IDeadLetterQueueNameFormatter
{
    public string FormatDeadLetterQueueName(string queueName)
    {
        return queueName + "-dead-letter";
    }
}

public class ErrorQueueNameFormatter : IErrorQueueNameFormatter
{
    public string FormatErrorQueueName(string queueName)
    {
        return queueName + "-error";
    }
}


public class PrefixEntityNameFormatter : IEntityNameFormatter
{
    private readonly string _prefix;

    public PrefixEntityNameFormatter(string prefix)
    {
        _prefix = prefix;
    }

    public string FormatEntityName<T>()
    {
        return $"{_prefix}:{typeof(T).Name}";
    }
}