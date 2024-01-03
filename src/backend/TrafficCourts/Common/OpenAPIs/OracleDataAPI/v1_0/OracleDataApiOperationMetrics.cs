using System.Diagnostics.Metrics;
using TrafficCourts.Diagnostics;

namespace TrafficCourts.Common.OpenAPIs.OracleDataAPI.v1_0
{
    public class OracleDataApiOperationMetrics : OperationMetrics, IOracleDataApiOperationMetrics
    {
        public const string MeterName = "oracledataapi";

        public OracleDataApiOperationMetrics(IMeterFactory factory) : base(factory, MeterName, "Oracle Data Api")
        {
        }
    }
}
