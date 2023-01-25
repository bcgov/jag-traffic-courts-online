namespace BCGov.VirusScan.Api.Models;

public class VirusScanResult
{
    public VirusScanStatus Status { get; set; }

    /// <summary>
    /// Will be setif the <see cref="Status"/> is <see cref="VirusScanStatus.Error"/>, otherwise null.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Will be setif the <see cref="Status"/> is <see cref="VirusScanStatus.Infected"/>, otherwise null.
    /// </summary>
    public string? VirusName { get; set; }
}