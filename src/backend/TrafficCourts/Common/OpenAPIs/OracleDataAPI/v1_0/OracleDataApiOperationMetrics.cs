using System.Diagnostics.Metrics;
using TrafficCourts.Diagnostics;

namespace TrafficCourts.Common.OpenAPIs.OracleDataAPI.v1_0
{
    public class OracleDataApiOperationMetrics : OperationMetrics, IOracleDataApiOperationMetrics
    {
        public const string MeterName = "OracleDataApi";

        public OracleDataApiOperationMetrics(IMeterFactory factory) : base(factory, MeterName, "oracledataapi", "Oracle Data Api")
        {
        }
    }
}
