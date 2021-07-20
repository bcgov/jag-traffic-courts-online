using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common;
using TrafficCourts.Common.Contract;

namespace DisputeApi.Web.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class OffenceDisputeDetail
    {
        public TrafficCourts.Common.Contract.OffenceAgreementStatus OffenceAgreementStatus { get; set; }
        public bool RequestReduction { get; set; }
        public bool RequestMoreTime { get; set; }
        public string ReductionReason { get; set; }
        public string MoreTimeReason { get; set; }
        public DisputeStatus Status { get; set; }
    }
}
