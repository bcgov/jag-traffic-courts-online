using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Models
{
    public class OffenceDisputeDetail
    {
        public TrafficCourts.Common.Contract.OffenceAgreementStatus OffenceAgreementStatus { get; set; }
        public bool RequestReduction { get; set; }
        public bool RequestMoreTime { get; set; }
        public string ReductionReason { get; set; }
        public string MoreTimeReason { get; set; }
        public TrafficCourts.Common.Contract.DisputeStatus Status { get; set; }
    }
}
