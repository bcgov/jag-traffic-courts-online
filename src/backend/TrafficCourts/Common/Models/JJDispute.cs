using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

/// <summary>
/// An extension of the JJDispute object to include dictionary of KEY IDs (Unique identifiers for the files in COMS) and filenames as VALUE of the saved files for this JJDispute.
/// </summary>
public partial class JJDispute
{
    /// <summary>
    /// Dictionary of IDs and Filenames of all the uploaded documents related to this particular JJDispute
    /// </summary>
    public Dictionary<Guid, string>? FileData { get; set; }
}
