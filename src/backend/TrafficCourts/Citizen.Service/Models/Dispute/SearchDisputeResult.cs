using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class SearchDisputeResult
    {
        [JsonPropertyName("dispute_id")]
        public string? DisputeId { get; set; }

        [JsonPropertyName("dispute_status")]
        public Messaging.MessageContracts.DisputeStatus? DisputeStatus { get; set; }

        [JsonPropertyName("is_error")]
        public bool IsError { get; set; }
    }
}
