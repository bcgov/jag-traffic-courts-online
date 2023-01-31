using BCGov.VirusScan.Api.Features;
using BCGov.VirusScan.Api.Models;
using System;
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

    private static readonly Timer _versionDuration;
    private static readonly Counter<long> _versionErrorTotal;

    static Instrumentation()
    {
        _meter = new Meter(MeterName);

        _scanDuration = new Timer(_meter, "scan.duration", "ms", "Elapsed time spent scanning files");
        _infectedFilesTotal = _meter.CreateCounter<long>("files.infected", "ea", "Number of infected files detected");
        _scannedErrorTotal = _meter.CreateCounter<long>("scan.errors", "ea", "Number of files that could not be scanned due to an error");

        _pingDuration = new Timer(_meter, "ping.duration", "ms", "Elapsed time spent sending pings to ClamAV");
        _pingErrorTotal = _meter.CreateCounter<long>("ping.errors", "ea", "Number of times ping to ClamAV failed");

        _versionDuration = new Timer(_meter, "version.duration", "ms", "Elapsed time spent sending version commands to ClamAV");
        _versionErrorTotal = _meter.CreateCounter<long>("version.errors", "ea", "Number of times ClamAV version command failed");
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

    public static void ScanInvalidResponse() => InvalidResponse(_scannedErrorTotal);

    #endregion

    #region Ping

    /// <summary>
    /// Records the start of a ping operation.
    /// </summary>
    /// <returns>The timer operation.</returns>

    public static ITimerOperation BeginPing()
    {
        return _pingDuration.Start();
    }

    public static void EndPing(ITimerOperation operation, bool pong)
    {
        operation.AddTag("success", pong);
    }

    /// <summary>
    /// The ping operation ended due to an exception.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="exception"></param>
    public static void EndPing(ITimerOperation operation, Exception exception)
    {
        End(_pingErrorTotal, operation, exception);
    }

    public static void PingInvalidResponse() => InvalidResponse(_pingErrorTotal);

    #endregion


    #region Get Version
    /// <summary>
    /// Records the start of a get version operation.
    /// </summary>
    /// <returns>The timer operation.</returns>
    public static ITimerOperation BeginGetVersion()
    {
        return _versionDuration.Start();
    }

    /// <summary>
    /// The get version operation ended with a result.
    /// </summary>
    public static void EndGetVersion(ITimerOperation operation, GetVersionResult version)
    {
        if (version is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(version.Version)) operation.AddTag("software-version", version.Version);
        if (version.Definition is not null)
        {
            if (!string.IsNullOrEmpty(version.Definition.Version)) operation.AddTag("definitions-version", version.Definition.Version);
            operation.AddTag("definitions-date", version.Definition.Date.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }

    /// <summary>
    /// The get version operation ended due to an exception.
    /// </summary>

    public static void EndGetVersion(ITimerOperation operation, Exception exception)
    {
        End(_versionErrorTotal, operation, exception);
    }

    /// <summary>
    /// The response returned by Clam AV to a VERSION command could not be parsed.
    /// </summary>
    public static void VersionInvalidResponse() => InvalidResponse(_versionErrorTotal);
    #endregion 

    /// <summary>
    /// Invalid response was detected.
    /// </summary>
    private static void InvalidResponse(Counter<long> errorCounter)
    {
        TagList tagList = new()
        {
            { "error_type", "Invalid Response" }
        };

        errorCounter.Add(1, tagList);

    }

    /// <summary>
    /// The operation ended due to an exception.
    /// </summary>
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

