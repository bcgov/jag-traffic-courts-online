using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Configuration
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class RabbitMQConfiguration
    {
        public RabbitMQConfiguration()
        {
            this.Host = "localhost";
            this.Port = 5672;
            this.Username = "guest";
            this.Password = "guest";
        }

        /// <summary>
        /// RabbitMq Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// RabbitMq Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// RabbitMq Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// RabbitMq Password
        /// </summary>
        public string Password { get; set; }
    }
}
