using BCGov.VirusScan.Api.Contracts.Version;
using BCGov.VirusScan.Api.Models;

namespace BCGov.VirusScan.Api.Contracts.Scan;

public class ScanRequest
{
    /// <summary>
    /// The file to scan for viruses
    /// </summary>
    public IFormFile? File { get; set; }
}


public class ScanResponse
{
    public VirusScanStatus Status { get; set; }
    public string? VirusName { get; set; }
}
