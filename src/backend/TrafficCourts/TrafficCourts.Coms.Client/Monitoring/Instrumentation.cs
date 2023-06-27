using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace TrafficCourts.Coms.Client.Monitoring;

internal static class Instrumentation
{
    public const string MeterName = "ComsClient";

    private static readonly Meter _meter;

    private static readonly Timer _operationDuration;
    private static readonly Counter<long> _operationErrorTotal;

    static Instrumentation()
    {
        _meter = new Meter(MeterName);

        _operationDuration = new Timer(_meter, "coms.operation.duration", "ms", "Elapsed time spent executing a COMS operation");
        _operationErrorTotal = _meter.CreateCounter<long>("coms.operation.errors", "ea", "Number of times a COMS operation not be completed due to an error");
    }

    /// <summary>
    /// Indicates the start of an operation.
    /// </summary>
    /// <param name="operation">The name of the operation.</param>
    /// <returns>A disposable timer operation.</returns>
    public static ITimerOperation BeginOperation(string operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        // caller with with Async operation name
        if (operation.EndsWith("Async"))
        {
            operation = operation[..^5];
        }
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
}
