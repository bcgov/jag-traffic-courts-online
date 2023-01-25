using BCGov.VirusScan.Api.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BCGov.VirusScan.Api.Monitoring;

public static class Instrumentation
{
    public static readonly ActivitySource Source = new ActivitySource("bc-gov-virus-scan");

    public const string MeterName = "VirusScan";

    static Meter _meter;

    // scan instruments
    private static readonly Timer _scanDuration;
    private static readonly Counter<long> _infectedFilesTotal;
    private static readonly Counter<long> _scannedErrorTotal;

    // ping instruments
    private static readonly Timer _pingDuration;
    private static readonly Counter<long> _pingErrorTotal;

    static Instrumentation()
    {
        _meter = new Meter(MeterName);

        _scanDuration = new Timer(_meter, "scan.duration", "ms", "Elapsed time spent scanning files");
        _infectedFilesTotal = _meter.CreateCounter<long>("files.infected", "ea", "Number of infected files detected");
        _scannedErrorTotal = _meter.CreateCounter<long>("scan.errors", "ea", "Number of files that could not be scanned due to an error");

        _pingDuration = new Timer(_meter, "ping.duration", "ms", "Elapsed time spent sending pings to ClamAV");
        _pingErrorTotal = _meter.CreateCounter<long>("ping.errors", "ea", "Number of times ping to ClamAV failed");
    }

    #region Virus Scan
    /// <summary>
    /// Starts timer and increments the number of files scanned.
    /// </summary>
    /// <returns>The timer operation</returns>
    public static ITimerOperation BeginVirusScan()
    {
        return _scanDuration.Start();
    }

    public static void EndVirusScan(VirusScanStatus status)
    {
        if (status == VirusScanStatus.Infected)
        {
            _infectedFilesTotal.Add(1);
        }
    }

    /// <summary>
    /// Ends the operation with error.
    /// </summary>
    public static void EndVirusScan(ITimerOperation operation, Exception exception)
    {
        End(_scannedErrorTotal, operation, exception);
    }

    #endregion

    #region Ping

    public static ITimerOperation BeginPing()
    {
        return _pingDuration.Start();
    }

    public static void EndPing()
    {
    }

    /// <summary>
    /// Ends the operation with error.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="exception"></param>
    public static void EndPing(ITimerOperation operation, Exception exception)
    {
        End(_pingErrorTotal, operation, exception);
    }

    #endregion

    private static void End(Counter<long> errorTotal, ITimerOperation operation, Exception exception)
    {
        // let the timer know there was an excetion
        operation.Error(exception);

        TagList tagList = new()
        {
            { "exception_type", exception.GetType().Name }
        };

        // increment the error counter
        errorTotal.Add(1, tagList);
    }
}

