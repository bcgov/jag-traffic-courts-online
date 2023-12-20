using System.Diagnostics.Metrics;
using TrafficCourts.Diagnostics;

namespace TrafficCourts.Common.OpenAPIs.OracleDataAPI.v1_0
{
    public class OracleDataApiOperationMetrics : OperationMetrics, IOracleDataApiOperationMetrics
    {
        public OracleDataApiOperationMetrics(IMeterFactory factory, string meterName) : base(factory, meterName, "oracledataapi", "Oracle Data Api")
        {
        }
    }
}
