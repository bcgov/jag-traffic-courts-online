namespace TrafficCourts.Messaging.Configuration
{
    /// <summary>
    /// Provides properties for configuring MassTransit.
    /// </summary>
    public interface IMassTransitConfiguration
    {
        MassTransitConfigurationProperties? MassTransit { get; set; }
    }
}
