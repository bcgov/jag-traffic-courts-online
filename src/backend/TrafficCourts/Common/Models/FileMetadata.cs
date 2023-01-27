using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Models;

/// <summary>
/// A class that contains metadata for a file that was uploaded through COMS
/// </summary>
[ExcludeFromCodeCoverage]
public class FileMetadata
{
    /// <summary>
    /// Unique identifier for a file in COMS
    /// </summary>
    public Guid FileId { get; set; }

    /// <summary>
    /// The file name of the uploaded file
    /// </summary>
    public string FileName { get; set; } = String.Empty;
}
