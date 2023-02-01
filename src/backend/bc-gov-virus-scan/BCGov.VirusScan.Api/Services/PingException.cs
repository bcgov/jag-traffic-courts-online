namespace BCGov.VirusScan.Api.Services;

public class PingException : VirusScanServiceException
{
    public PingException(Exception innerException) : base("Error pinning virus scan server", innerException)
    {
    }
}
