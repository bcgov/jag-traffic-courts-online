namespace TrafficCourts.Workflow.Service.Services;

[Serializable]
public class SmtpConnectFailedException : Exception
{
    public SmtpConnectFailedException(string? message, Exception? innerException) : base(message, innerException) { }
 }
