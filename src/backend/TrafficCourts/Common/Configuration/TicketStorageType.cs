namespace TrafficCourts.Common.Configuration;

public enum TicketStorageType
{
    /// <summary>
    /// Unknown type (undefined). Must be index 0.
    /// </summary>
    Unknown,
    ObjectStore,
    InMemory
}
