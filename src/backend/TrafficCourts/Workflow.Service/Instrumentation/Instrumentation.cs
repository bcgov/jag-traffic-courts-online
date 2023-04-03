using System.Diagnostics;
using System.Diagnostics.Metrics;
using TrafficCourts.Common.Diagnostics;
using Timer = TrafficCourts.Common.Diagnostics.Timer;

namespace TrafficCourts.Workflow.Service;

public static class Instrumentation
{
    public const string MeterName = "WorkflowService";

    private static readonly Meter _meter;

    private static readonly Timer _oracleDataApiOperation;
    private static readonly Counter<long> _oracleDataApiOperationErrorTotal;

    private static readonly Timer _smtpOperation;
    private static readonly Counter<long> _smtpOperationErrorTotal;

    static Instrumentation()
    {
        _meter = new Meter(MeterName);

        _oracleDataApiOperation = new Timer(_meter, "oracle_data_api.operation.duration", "ms", "Elapsed time spent executing a Oracle Data Api operation");
        _oracleDataApiOperationErrorTotal = _meter.CreateCounter<long>("oracle_data_api.operation.errors", "ea", "Number of times a Oracle Data Api operation not be completed due to an error");

        _smtpOperation = new Timer(_meter, "smtp.operation.duration", "ms", "Elapsed time spent executing a smtp operation");
        _smtpOperationErrorTotal = _meter.CreateCounter<long>("smtp.operation.errors", "ea", "Number of times a smtp operation not be completed due to an error");
    }

    public static class OracleDataApi
    {
        public static ITimerOperation BeginOperation(string operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
            return _oracleDataApiOperation.Start(new TagList { { "operation", operation } });
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
            _oracleDataApiOperationErrorTotal.Add(1, operation.Tags);
        }
    }


    public static class Smtp
    {
        public static ITimerOperation BeginOperation(string operation)
        {
            ArgumentNullException.ThrowIfNull(operation);
            return _smtpOperation.Start(new TagList { { "operation", operation } });
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
            _smtpOperationErrorTotal.Add(1, operation.Tags);
        }
    }
}
