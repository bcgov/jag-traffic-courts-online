namespace BCGov.VirusScan.Api.Services;

public class VersionException : VirusScanServiceException
{
    public VersionException(Exception innerException) : base("Error getting virus scan version", innerException)
    {
    }
}
