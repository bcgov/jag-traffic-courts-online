namespace BCGov.VirusScan.Api.Models;

public enum VirusScanStatus
{
    /// <summary>
    /// There was an error scanning the file.
    /// </summary>
    Error = 1,

    /// <summary>
    /// The virus scan found a virus.
    /// </summary>
    Infected = 2,

    /// <summary>
    /// The virus scan passed. No viruses found.
    /// </summary>
    NotInfected = 3,
}
