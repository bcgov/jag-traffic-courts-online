using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Messaging.Configuration
{
    /// <summary>
    /// Provides convenience environment variables for configuring RabbitMQ connections.
    /// </summary>
    public class RabbitMQConfigurationProvider : TrafficCourtsConfigurationProvider
    {
        public override void Load()
        {
            Add("RABBITMQ_HOST", $"{nameof(IRabbitMQConfiguration.RabbitMQ)}:{nameof(RabbitMQConfigurationProperties.Host)}");
            Add("RABBITMQ_PORT", $"{nameof(IRabbitMQConfiguration.RabbitMQ)}:{nameof(RabbitMQConfigurationProperties.Port)}");
            Add("RABBITMQ_USERNAME", $"{nameof(IRabbitMQConfiguration.RabbitMQ)}:{nameof(RabbitMQConfigurationProperties.Username)}");
            Add("RABBITMQ_PASSWORD", $"{nameof(IRabbitMQConfiguration.RabbitMQ)}:{nameof(RabbitMQConfigurationProperties.Password)}");
        }
    }
}
