namespace TrafficCourts.Domain.Models;

/// <summary>
/// The status of a document.
/// </summary>
public enum DocumentStatus
{
    /// <summary>
    /// The document has been entered into the file and does not require additional action.
    /// </summary>
    Filed = 1,

    /// <summary>
    /// The document is awaiting action.
    /// </summary>
    Pending = 2,

    /// <summary>
    /// The document is no longer awaiting action.
    /// </summary>
    Resolved = 3,
}
