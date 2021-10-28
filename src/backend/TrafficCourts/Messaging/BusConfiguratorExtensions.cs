using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging.Configuration;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace TrafficCourts.Messaging;

public static class BusConfiguratorExtensions
{
    // Can add additional optional overloads of we need to customize various parts
    //
    // Action<IRabbitMqHostConfigurator>? configureHost = null,
    // Action<IRabbitMqBusFactoryConfigurator>? configureBusFactory = null

    public static void AddMassTransit<TConfiguration>(this IServiceCollection services, ConfigurationManager configurationManager)
        where TConfiguration : IMassTransitConfiguration, IRabbitMQConfiguration
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configurationManager == null) throw new ArgumentNullException(nameof(configurationManager));

        // map environment variables to configuration
        configurationManager.Add<RabbitMQConfigurationProvider>();

        // get the configuration type
        TConfiguration configuration = configurationManager.Get<TConfiguration>();

        services.AddMassTransit(config =>
        {
            switch (configuration.MassTransit.Transport)
            {
                case MassTransitTransport.InMemory:
                    UseInMemory(config);
                    break;

                case MassTransitTransport.RabbitMQ:
                    UseRabbitMq(config, configuration);
                    break;
                default:
                    throw new ConfigurationErrorsException($"Invalid MassTransit Transport: ${configuration.MassTransit.Transport}");
            }

            services.AddMassTransitHostedService();
        });
    }

    private static void UseInMemory(IServiceCollectionBusConfigurator config)
    {
        config.UsingInMemory((context, cfg) =>
        {
            // see https://masstransit-project.com/usage/transports/in-memory.html
            // TODO
            //cfg.TransportConcurrencyLimit = 100;
            //cfg.ConfigureEndpoints(context);
        });
    }

    private static void UseRabbitMq<TConfiguration>(IServiceCollectionBusConfigurator config, TConfiguration configuration)
        where TConfiguration : IRabbitMQConfiguration
    {
        RabbitMQConfigurationProperties? rabbitMQConfiguration = configuration.RabbitMQ;

        if (rabbitMQConfiguration == null)
        {
            throw new ConfigurationErrorsException();
        }

        // should this be rabbit:// ?
        var rabbitBaseUri = $"amqp://{rabbitMQConfiguration.Host}:{rabbitMQConfiguration.Port}";

        // see https://masstransit-project.com/usage/transports/rabbitmq.html
        config.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(new Uri(rabbitBaseUri), hostConfig =>
            {
                hostConfig.Username(rabbitMQConfiguration.Username);
                hostConfig.Password(rabbitMQConfiguration.Password);
            });
        });
    }
}
