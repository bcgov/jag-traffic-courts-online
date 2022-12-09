using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Messaging.MessageContracts;

public class SearchDisputeRequest
{
    public string TicketNumber { get; set; } = String.Empty;
    public string IssuedTime { get; set; } = String.Empty;
}

public class SearchDisputeResponse
{
    public string? DisputeId { get; set; }
    public string? DisputeStatus { get; set; }
    public string? JJDisputeStatus { get; set; }
    public bool IsError { get; set; }
}
