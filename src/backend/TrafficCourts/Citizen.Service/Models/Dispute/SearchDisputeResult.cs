using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class SearchDisputeResult
    {
        [JsonPropertyName("dispute_id")]
        public string? DisputeId { get; set; }

        [JsonPropertyName("dispute_status")]
        public DisputeStatus? DisputeStatus { get; set; }

        [JsonPropertyName("jjdispute_status")]
        public JJDisputeStatus? JJDisputeStatus { get; set; }

        [JsonPropertyName("is_error")]
        public bool IsError { get; set; }
    }
}
