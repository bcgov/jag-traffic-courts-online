using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Models.Disputes
{
    public class SearchDisputeResult
    {
        [JsonPropertyName("token")]
        public string? NoticeOfDisputeGuid { get; set; }

        [JsonPropertyName("dispute_status")]
        public DisputeStatus? DisputeStatus { get; set; }

        [JsonPropertyName("jjdispute_status")]
        public JJDisputeStatus? JJDisputeStatus { get; set; }

        [JsonPropertyName("hearing_type")]
        public JJDisputeHearingType? HearingType { get; set; }

        [JsonPropertyName("is_error")]
        public bool IsError { get; set; }
    }
}
