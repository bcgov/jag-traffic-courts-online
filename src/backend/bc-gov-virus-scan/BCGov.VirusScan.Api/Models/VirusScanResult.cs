namespace BCGov.VirusScan.Api.Models;

public class VirusScanResult
{
    /// <summary>
    /// The virus scan status.
    /// </summary>
    public VirusScanStatus Status { get; set; }

    /// <summary>
    ///  If the status is Error, the error message , otherwise null.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// If the status is Infected, the virus name, otherwise null.
    /// </summary>
    public string? VirusName { get; set; }
}