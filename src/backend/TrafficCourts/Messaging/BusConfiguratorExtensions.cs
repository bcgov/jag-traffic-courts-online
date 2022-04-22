using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Utils;
using TrafficCourts.Messaging.Configuration;

namespace TrafficCourts.Messaging;

public static class BusConfiguratorExtensions
{
    public static void AddMassTransit<TConfiguration>(this IServiceCollection services, WebApplicationBuilder builder)
        where TConfiguration : IRabbitMQConfiguration
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        // get the configuration type
        TConfiguration configuration = builder.Configuration.Get<TConfiguration>();

        services.AddMassTransit(configure =>
        {
            UseRabbitMq(configure, configuration);
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

            cfg.ConfigureJsonSerializerOptions(settings =>
            {
                settings.Converters.Add(new DateOnlyJsonConverter());
                settings.Converters.Add(new TimeOnlyJsonConverter());
                return settings;
            });
        });
    }
}
