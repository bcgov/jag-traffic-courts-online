using BCGov.VirusScan.Api.Models;

namespace BCGov.VirusScan.Api.Contracts.Scan;

public class ScanResponse
{
    /// <summary>
    /// The virus scan status.
    /// </summary>
    public VirusScanStatus Status { get; set; }

    /// <summary>
    /// The virus name if the status is <see cref="VirusScanStatus.Infected"/>.
    /// </summary>
    public string? VirusName { get; set; }
}
