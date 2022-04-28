namespace TrafficCourts.Messaging.Configuration
{
    /// <summary>
    /// Defines the transport to use with MassTransit.
    /// </summary>
    public enum MassTransitTransport
    {
        /// <summary>
        /// Use Rabbit MQ transport.
        /// </summary>
        RabbitMq,

        /// <summary>
        /// Use In Memory transport in development.
        /// </summary>
        InMemory
    }
}
