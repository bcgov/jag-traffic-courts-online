using System.Diagnostics.Metrics;
using TrafficCourts.Diagnostics;

namespace TrafficCourts.OracleDataApi;

internal interface IOracleDataApiOperationMetrics : IOperationMetrics
{
}

internal class OracleDataApiOperationMetrics : OperationMetrics, IOracleDataApiOperationMetrics
{
    public const string MeterName = "OracleDataApi";

    public OracleDataApiOperationMetrics(IMeterFactory factory) : base(factory, MeterName, "oracledataapi", "Oracle Data Api")
    {
    }
}
