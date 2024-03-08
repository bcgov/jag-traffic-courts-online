namespace TrafficCourts.Citizen.Service;

public enum TicketSearchType
{
    /// <summary>
    /// Unknown type (undefined). Must be index 0.
    /// </summary>
    Unknown,

    /// <summary>
    /// Use mock data.
    /// </summary>
    Mock,

    /// <summary>
    /// Hybrid will use RoadSafety and if that errors out or returns not found
    /// the Mock will be used.
    /// </summary>
    Hybrid,

    /// <summary>
    /// Use Road Safety service
    /// </summary>
    RoadSafety
}
