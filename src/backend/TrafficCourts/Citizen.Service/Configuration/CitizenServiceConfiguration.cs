using TrafficCourts.Messaging.Configuration;

namespace TrafficCourts.Citizen.Service.Configuration;

public class CitizenServiceConfiguration : IRabbitMQConfiguration, IMassTransitConfiguration
{
    public RabbitMQConfigurationProperties? RabbitMQ { get; set; }
    public MassTransitConfigurationProperties? MassTransit { get; set; }
}
