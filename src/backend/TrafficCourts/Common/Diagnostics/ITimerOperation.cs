namespace TrafficCourts.Common.Diagnostics;

/// <summary>
/// Timer operation. When timer operation is disposed, the elapsed time will be recorded.
/// </summary>
public interface ITimerOperation : IDisposable
{
    void Error(Exception exception);
    void AddTag(string name, object value);
}
