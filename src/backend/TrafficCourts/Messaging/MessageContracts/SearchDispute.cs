namespace TrafficCourts.Messaging.MessageContracts;

public class SearchDisputeRequest
{
    public string? TicketNumber { get; set; } = String.Empty;
    public string? IssuedTime { get; set; } = String.Empty;
    public Guid? NoticeOfDisputeGuid { get; set; } = null!;
}

public class SearchDisputeResponse
{
    /// <summary>
    /// The notice of dispute identifer.
    /// </summary>
    public Guid? NoticeOfDisputeGuid { get; set; }
    public string? DisputeStatus { get; set; }
    public string? JJDisputeStatus { get; set; }
    public string? HearingType { get; set; }
    public bool IsError { get; set; }
}
