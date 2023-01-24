namespace BCGov.VirusScan.Api.Models;

public enum VirusScanStatus
{
    /// <summary>
    /// The result is unknown.
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// The virus scan passed. No viruses found.
    /// </summary>
    NotInfected,

    /// <summary>
    /// The virus scan found a virus.
    /// </summary>
    Infected,

    /// <summary>
    /// There was an error scanning the file.
    /// </summary>
    Error
}
