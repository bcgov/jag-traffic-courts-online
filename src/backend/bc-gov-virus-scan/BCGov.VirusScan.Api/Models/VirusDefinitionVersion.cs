namespace BCGov.VirusScan.Api.Models;

public class VirusDefinitionVersion
{
    public VirusDefinitionVersion(string version, DateTime date)
    {
        Version = version;
        Date = date;
    }

    /// <summary>
    /// The version of the virus definition signatures.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// The date of the virus definition signatures.
    /// </summary>
    public DateTime Date { get; }
}
