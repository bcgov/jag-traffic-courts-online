using System.Text.Json.Serialization;

namespace TrafficCourts.OrdsDataService.Occam;

public partial class OccamDisputeUpdateRequests
{
    [JsonPropertyName("dispute_update_stat_type_dsc")]
    public string? DisputeUpdateStatTypeDsc { get; set; }

    [JsonPropertyName("dispute_update_req_type_dsc")]
    public string? DisputeUpdateReqTypeDsc { get; set; }
}
