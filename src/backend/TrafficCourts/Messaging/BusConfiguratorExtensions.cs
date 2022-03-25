using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging.Configuration;
using TrafficCourts.Messaging.MessageContracts;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;

namespace TrafficCourts.Messaging;

public static class BusConfiguratorExtensions
{
    // Can add additional optional overloads of we need to customize various parts
    //
    // Action<IRabbitMqHostConfigurator>? configureHost = null,
    // Action<IRabbitMqBusFactoryConfigurator>? configureBusFactory = null

    public static void AddMassTransit<TConfiguration>(this IServiceCollection services, WebApplicationBuilder builder)
        where TConfiguration : IMassTransitConfiguration, IRabbitMQConfiguration
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        ConfigurationManager configurationManager = builder.Configuration;
        // map environment variables to configuration
        configurationManager.Add<RabbitMQConfigurationProvider>();

        // get the configuration type
        TConfiguration configuration = configurationManager.Get<TConfiguration>();

        ApplyDevelopmentDefaults(builder, configuration);

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
        });
    }

    private static void ApplyDevelopmentDefaults<TConfiguration>(WebApplicationBuilder builder, TConfiguration configuration)
        where TConfiguration : IMassTransitConfiguration
    {
        if (builder.Environment.IsDevelopment())
        {
            if (configuration.MassTransit is null)
            {
                configuration.MassTransit = new MassTransitConfigurationProperties { Transport = MassTransitTransport.InMemory };
            }
        }
    }

    private static void UseInMemory(IBusRegistrationConfigurator config)
    {
        config.UsingInMemory((context, cfg) =>
        {
            // see https://masstransit-project.com/usage/transports/in-memory.html
            // TODO
            //cfg.TransportConcurrencyLimit = 100;
            //cfg.ConfigureEndpoints(context);
        });
    }

    private static void UseRabbitMq<TConfiguration>(IBusRegistrationConfigurator config, TConfiguration configuration)
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

        config.AddRequestClient<SubmitDispute>();
    }
}
