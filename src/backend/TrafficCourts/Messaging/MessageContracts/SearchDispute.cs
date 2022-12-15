using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Messaging.MessageContracts;

public class SearchDisputeRequest
{
    public string TicketNumber { get; set; } = String.Empty;
    public string IssuedTime { get; set; } = String.Empty;
}

public class SearchDisputeResponse
{
    /// <summary>
    /// The notice of dispute identifer.
    /// </summary>
    public string? NoticeOfDisputeGuid { get; set; }
    public string? DisputeStatus { get; set; }
    public string? JJDisputeStatus { get; set; }
    public string? HearingType { get; set; }
    public bool IsError { get; set; }
}
