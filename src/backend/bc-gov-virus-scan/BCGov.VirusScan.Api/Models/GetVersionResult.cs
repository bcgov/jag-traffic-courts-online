namespace BCGov.VirusScan.Api.Models;

/// <summary>
/// The virus scanner version details.
/// </summary>
public class GetVersionResult
{
    public GetVersionResult(string version, string? databaseVersion, DateTime? databaseDate)
    {
        Version = version;
        if (!string.IsNullOrEmpty(databaseVersion) && databaseDate is not null)
        {
            Definition = new VirusDefinitionVersion(databaseVersion, databaseDate.Value);
        }
    }

    /// <summary>
    /// The version of the Clam AV server software. 
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// The virus definition version. If the Clam AV server has not downloaded the definitions, this will be null.
    /// </summary>
    public VirusDefinitionVersion? Definition { get; set; }
}
