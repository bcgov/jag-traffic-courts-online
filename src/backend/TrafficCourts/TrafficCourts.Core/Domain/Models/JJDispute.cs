namespace TrafficCourts.Domain.Models;

/// <summary>
/// An extension of the JJDispute object to include list of file metadata that contain ID (Unique identifiers for the files in COMS) and filename of the saved files for this JJDispute.
/// </summary>
public partial class JJDispute
{
    /// <summary>
    /// List of file metadata that contain ID and Filename of all the uploaded documents related to this particular JJDispute
    /// </summary>
    public List<FileMetadata>? FileData { get; set; }

    /// <summary>
    /// ID of the read/write Lock acquired by the user
    /// </summary>
    public string? LockId { get; set; }

    /// <summary>
    /// Username of the user who acquired read/write lock on the dispute
    /// </summary>
    public string? LockedBy { get; set; }

    /// <summary>
    /// The time in UTC when the acquired dispute user lock expires.
    /// </summary>
    public DateTimeOffset? LockExpiresAtUtc { get; set; }
}
