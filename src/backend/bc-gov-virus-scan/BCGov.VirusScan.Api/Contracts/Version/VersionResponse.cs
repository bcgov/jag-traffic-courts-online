namespace BCGov.VirusScan.Api.Contracts.Version;

public class VersionResponse
{
    /// <summary>
    /// The version of the Clam AV server.
    /// </summary>
    public string SoftwareVersion { get; }

    /// <summary>
    /// The version of the signatures.
    /// </summary>
    public string? DatabaseVersion { get; }
    /// <summary>
    /// The date of the signatures.
    /// </summary>
    public DateTime? DatabaseDate { get; }

    public VersionResponse(string softwareVersion, string? databaseVersion, DateTime? databaseDate)
    {
        SoftwareVersion = softwareVersion;
        DatabaseVersion = databaseVersion;
        DatabaseDate = databaseDate;
    }
}
