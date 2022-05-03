namespace TrafficCourts.Citizen.Service.Configuration;

/// <summary>
/// The citizen service configuration.
/// </summary>
public class CitizenServiceConfiguration
{
    public TicketStorageType TicketStorage { get; set; } = TicketStorageType.InMemory;
}
