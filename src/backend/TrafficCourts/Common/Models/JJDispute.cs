using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

/// <summary>
/// An extension of the JJDispute object to include list of file metadata that contain ID (Unique identifiers for the files in COMS) and filename of the saved files for this JJDispute.
/// </summary>
public partial class JJDispute
{
    /// <summary>
    /// List of file metadata that contain ID and Filename of all the uploaded documents related to this particular JJDispute
    /// </summary>
    public List<FileMetadata>? FileData { get; set; }
}
