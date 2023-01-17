using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

/// <summary>
/// An extension of the JJDispute object to include list of IDs (Unique identifiers for the files in COMS) of the saved files for this JJDispute.
/// </summary>
public partial class JJDispute
{
    /// <summary>
    /// List of IDs of all the uploaded documents related to this particular JJDispute
    /// </summary>
    public List<Guid>? FileIds { get; set; }
}
