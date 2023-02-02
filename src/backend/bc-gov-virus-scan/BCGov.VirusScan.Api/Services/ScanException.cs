namespace BCGov.VirusScan.Api.Services;

public class ScanException : VirusScanServiceException
{
    public ScanException(Exception innerException) : base("Error virus scanning file", innerException)
    {
    }
}
