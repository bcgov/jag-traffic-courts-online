using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Messaging.Configuration
{
    public class RabbitMQConfigurationProperties
    {
        /// <summary>
        /// RabbitMQ Host
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// RabbitMQ Port
        /// </summary>
        [Range(ushort.MinValue, ushort.MaxValue)]
        public int Port { get; set; } = 5672;

        /// <summary>
        /// RabbitMQ Username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// RabbitMQ Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
