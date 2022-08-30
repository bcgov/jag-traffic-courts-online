using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class DisputeCount
    {
        /// <summary>
        /// Represents the dispuant plea on count.
        /// </summary>
        [JsonPropertyName("plea_cd")]
        public DisputeCountPleaCode PleaCode { get; set; }

        /// <summary>
        /// The count number. Must be unique within an individual dispute.
        /// </summary>
        [JsonPropertyName("count_no")]
        public short CountNo { get; set; }

        /// <summary>
        /// The disputant is requesting time to pay the ticketed amount.
        /// </summary>
        [JsonPropertyName("request_time_to_pay")]
        public DisputeCountRequestTimeToPay? RequestTimeToPay { get; set; }

        /// <summary>
        /// The disputant is requesting a reduction of the ticketed amount.
        /// </summary>
        [JsonPropertyName("request_reduction")]
        public DisputeCountRequestReduction? RequestReduction { get; set; }

        /// <summary>
        /// Does the want to appear in court?
        /// </summary>
        [JsonPropertyName("request_court_appearance")]
        public DisputeCountRequestCourtAppearance? RequestCourtAppearance { get; set; }
    }

}
