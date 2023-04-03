using Azure;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using TrafficCourts.Common.Diagnostics;
using Timer = TrafficCourts.Common.Diagnostics.Timer;

namespace TrafficCourts.Citizen.Service;

public class Instrumentation
{
    public const string MeterName = "CitizenService";

    private static readonly Meter _meter;

    private static readonly Timer _formRecognizerOperation;
    private static readonly Counter<long> _formRecognizerOperationErrorTotal;

    static Instrumentation()
    {
        _meter = new Meter(MeterName);

        //// Form Recognizer operations
        _formRecognizerOperation = new Timer(_meter, "formrecognizer.operation.duration", "ms", "Elapsed time spent executing a Form Recognizer operation");
        _formRecognizerOperationErrorTotal = _meter.CreateCounter<long>("formrecognizer.operation.errors", "ea", "Number of times a Form Recognizer operation not be completed due to an error");
    }

    public static class FormRecognizer
    {
        public static ITimerOperation BeginOperation(string version, string operation)
        {
            ArgumentNullException.ThrowIfNull(version);
            ArgumentNullException.ThrowIfNull(operation);

            // caller with with Async operation name
            if (operation.EndsWith("Async"))
            {
                operation = operation[..^5];
            }

            return _formRecognizerOperation.Start(new TagList {
                { "operation", operation },
                { "version", version }
            });
        }

        public static void EndOperation(ITimerOperation operation, Exception exception)
        {
            ArgumentNullException.ThrowIfNull(operation);
            ArgumentNullException.ThrowIfNull(exception);

            // let the timer know there was an excetion
            operation.Error(exception);

            // increment the error counter and record the same tags as the operation
            _formRecognizerOperationErrorTotal.Add(1, operation.Tags);
        }
    }

}
}
