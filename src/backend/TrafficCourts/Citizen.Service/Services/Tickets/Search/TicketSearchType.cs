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
    /// Use Road Safety service
    /// </summary>
    RoadSafety
}
