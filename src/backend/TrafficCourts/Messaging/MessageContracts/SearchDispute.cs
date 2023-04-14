using Microsoft.AspNetCore.Http.HttpResults;

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

    /// <summary>
    /// Indicates there was an error searching for the dispute.
    /// If <c>true</c>, <see cref="IsNotFound"/> must be false and the other properties null.
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// Indicates the response is not found. 
    /// If <c>true</c>, <see cref="IsError"/> must be false and the other properties null.
    /// </summary>
    public bool IsNotFound { get; set; }

    /// <summary>
    /// Returns a <see cref="SearchDisputeResponse"/> indicating the dispute was not found.
    /// </summary>
    public static readonly SearchDisputeResponse NotFound = new()
    {
        IsError = false,
        IsNotFound = true
    };

    /// <summary>
    /// Returns a <see cref="SearchDisputeResponse"/> indicating and error occurred searching for the dispute.
    /// </summary>
    public static readonly SearchDisputeResponse Error = new()
    {
        IsError = true,
        IsNotFound = false
    };
}
