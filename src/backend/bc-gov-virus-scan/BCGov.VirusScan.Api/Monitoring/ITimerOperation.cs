namespace BCGov.VirusScan.Api.Monitoring;

public interface ITimerOperation : IDisposable
{
    void Error(Exception exception);
    void AddTag(string name, object value);
}
