using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class DisputeCount
    {
        /// <summary>
        /// Represents the dispuant plea on count.
        /// </summary>
        public DisputeCountPleaCode PleaCode { get; set; }

        /// <summary>
        /// Count No
        /// </summary>
        public int CountNo { get; set; }

        /// <summary>
        /// The disputant is requesting time to pay the ticketed amount.
        /// </summary>
        public DisputeCountRequestTimeToPay RequestTimeToPay { get; set; } = DisputeCountRequestTimeToPay.N;

        /// <summary>
        /// The disputant is requesting a reduction of the ticketed amount.
        /// </summary>
        public DisputeCountRequestReduction RequestReduction { get; set; } = DisputeCountRequestReduction.N;

        /// <summary>
        /// Does the want to appear in court?
        /// </summary>
        public DisputeCountRequestCourtAppearance RequestCourtAppearance { get; set; } = DisputeCountRequestCourtAppearance.N;
    }
}
