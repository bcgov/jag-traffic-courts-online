namespace BCGov.VirusScan.Api.Services;

public abstract class VirusScanServiceException : Exception
{
    protected VirusScanServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
