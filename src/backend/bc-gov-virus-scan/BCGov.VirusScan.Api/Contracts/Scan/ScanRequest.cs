using BCGov.VirusScan.Api.Contracts.Version;

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
    public VersionResponse? Version { get; set; }
}
