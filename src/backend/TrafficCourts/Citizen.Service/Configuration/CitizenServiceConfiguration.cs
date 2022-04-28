using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging.Configuration;

namespace TrafficCourts.Citizen.Service.Configuration;

/// <summary>
/// The citizen service configuration.
/// </summary>
public class CitizenServiceConfiguration : 
    IRedisConfiguration
{
    public TicketStorageType TicketStorage { get; set; } = TicketStorageType.InMemory;
    public RedisConfigurationProperties? Redis { get; set; }
}
