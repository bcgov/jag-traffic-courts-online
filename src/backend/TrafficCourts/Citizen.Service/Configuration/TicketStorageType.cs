namespace TrafficCourts.Citizen.Service.Configuration;

public enum TicketStorageType
{
    /// <summary>
    /// Unknown type (undefined). Must be index 0.
    /// </summary>
    Unknown,
    InMemory,
    ObjectStore
}
