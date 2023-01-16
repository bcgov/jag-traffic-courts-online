namespace BCGov.VirusScan.Api.Models;

public class VirusScannerVersion
{
    public VirusScannerVersion(string softwareVersion, string? databaseVersion, DateTime? databaseDate)
    {
        SoftwareVersion = softwareVersion;
        DatabaseVersion = databaseVersion;
        DatabaseDate = databaseDate;
    }

    public string SoftwareVersion { get; }

    public string? DatabaseVersion { get; }

    public DateTime? DatabaseDate { get; }
}
