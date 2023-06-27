using System.Diagnostics;

namespace BCGov.VirusScan.Api.Monitoring;

public interface ITimerOperation : IDisposable
{
    /// <summary>
    /// Indicate the operation resulted in an error adding the appropriate tags to the operation.
    /// </summary>
    /// <param name="exception"></param>
    void Error(Exception exception);

    /// <summary>
    /// Gets a copy of the operation's tags.
    /// </summary>
    TagList Tags { get; }
}
