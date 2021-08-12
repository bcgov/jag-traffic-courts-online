using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common;
using TrafficCourts.Common.Contract;

namespace Gov.CitizenApi.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class OffenceDisputeDetail
    {
        public string OffenceAgreementStatus { get; set; }
        public bool RequestReduction { get; set; }
        public bool RequestMoreTime { get; set; }
        public bool? ReductionAppearInCourt { get; set; }
        public string ReductionReason { get; set; }
        public string MoreTimeReason { get; set; }
        public DisputeStatus Status { get; set; }
    }
}
