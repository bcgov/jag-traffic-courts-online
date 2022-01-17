namespace TrafficCourts.Messaging.Configuration
{
    /// <summary>
    /// Provides properties for configuring RabbitMQ.
    /// </summary>
    public interface IRabbitMQConfiguration
    {
        RabbitMQConfigurationProperties? RabbitMQ { get; set; }
    }
}
