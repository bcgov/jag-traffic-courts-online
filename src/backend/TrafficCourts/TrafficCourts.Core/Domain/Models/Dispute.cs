namespace TrafficCourts.Domain.Models;

/// <summary>
/// An extension of the Dispute object to include list of file metadata that contain ID (Unique identifiers for the files in COMS) and filename of the saved files for this Dispute.
/// </summary>
public partial class Dispute
{
    /// <summary>
    /// List of file metadata that contain ID and Filename of all the uploaded documents related to this particular Dispute
    /// </summary>
    public List<FileMetadata>? FileData { get; set; }
}

