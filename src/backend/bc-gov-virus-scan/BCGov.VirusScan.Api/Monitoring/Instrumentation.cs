using BCGov.VirusScan.Api.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BCGov.VirusScan.Api.Monitoring;

public static class Instrumentation
{
    public static readonly ActivitySource Source = new ActivitySource("bc-gov-virus-scan");

    public const string MeterName = "VirusScan";

    static Meter _meter;

    private static readonly Timer _operationDuration;
    private static readonly Counter<long> _operationErrorTotal;

    // scan instruments
    private static readonly Counter<long> _infectedFilesTotal;

    static Instrumentation()
    {
        _meter = new Meter(MeterName);

        _operationDuration = new Timer(_meter, "clamav.operation.duration", "ms", "Elapsed time spent on an ClamAV operation");
        _operationErrorTotal = _meter.CreateCounter<long>("clamav.operation.errors", "ea", "Number of operations that could not be completed due to an error");

        _infectedFilesTotal = _meter.CreateCounter<long>("files.infected", "ea", "Number of infected files detected");
    }

    public static ITimerOperation BeginOperation(string operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        return _operationDuration.Start(new TagList { { "operation", operation } });
    }

    /// <summary>
    /// Indicates an operation ended with an error.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="exception"></param>
    public static void EndOperation(ITimerOperation operation, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(exception);

        // let the timer know there was an excetion
        operation.Error(exception);

        // increment the error counter and record the same tags as the operation
        _operationErrorTotal.Add(1, operation.Tags);
    }

    /// <summary>
    /// Ends the operation with the virus scan status.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="status"></param>
    public static void EndOperation(ITimerOperation operation, VirusScanStatus status)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (status == VirusScanStatus.Infected)
        {
            _infectedFilesTotal.Add(1);
        }
    }

    /// <summary>
    /// Invalid response was detected.
    /// </summary>
    public static void InvalidResponse(ITimerOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        // get the same tags from the operation
        var tags = operation.Tags;
        tags.Add("error_type", "Invalid Response");
        _operationErrorTotal.Add(1, tags);
    }
}
