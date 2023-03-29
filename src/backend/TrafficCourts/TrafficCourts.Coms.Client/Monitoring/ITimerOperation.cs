namespace TrafficCourts.Coms.Client.Monitoring;

internal interface ITimerOperation : IDisposable
{
    void Error(Exception exception);
    void AddTag(string name, object value);
}
