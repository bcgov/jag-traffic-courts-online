namespace TrafficCourts.Common.Models;

/// <summary>
/// The kind of document properties, citizen or staff properties.
/// </summary>
public enum DocumentSource
{
    /// <summary>
    /// The document was uploaded by the citizen.
    /// </summary>
    Citizen = 1,

    /// <summary>
    /// The document was uploaded by staff.
    /// </summary>
    Staff = 2
}
