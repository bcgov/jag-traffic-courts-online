using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging.Configuration;

namespace TrafficCourts.Citizen.Service.Configuration;

/// <summary>
/// The citizen service configuration.
/// </summary>
public class CitizenServiceConfiguration : 
    IRabbitMQConfiguration, 
    ITicketSearchServiceConfiguration,
    IRedisConfiguration
{
    public RabbitMQConfigurationProperties? RabbitMQ { get; set; }
    public FormRecognizerConfigurationOptions? FormRecognizer { get; set; }
    public TicketSearchServiceConfigurationProperties? TicketSearchClient { get; set; }
    public TicketStorageType TicketStorage { get; set; } = TicketStorageType.InMemory;
    public RedisConfigurationProperties? Redis { get; set; }
}

public enum TicketStorageType
{
    InMemory,
    ObjectStore
}
