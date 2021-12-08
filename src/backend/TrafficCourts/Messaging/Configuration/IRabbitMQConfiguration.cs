namespace TrafficCourts.Messaging.Configuration
{
    public interface IRabbitMQConfiguration
    {
        RabbitMQConfigurationProperties? RabbitMQ { get; set; }
    }

    public interface IMassTransitConfiguration
    {
        MassTransitConfigurationProperties MassTransit { get; set; }
    }
}
