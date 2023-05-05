using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// A subset of a Disputant's contact information that can be requested to update via a PUT /api/dispute/{guidhash}/contact endpoint.
/// </summary>
public class DisputeUpdateRequestGetDispute
{
    /// <summary>
    /// The unique system generated notice of dispute identifer.
    /// </summary>
    public Guid NoticeOfDisputeGuid { get; set; }

    /// <summary>
    /// BCSC Token used to gain access to dispute record
    /// </summary>
    public Guid BCSCToken { get; set; }

    /// <summary>
    /// The violation ticket number.
    /// </summary>
    public string TicketNumber { get; set; } = null!;

    /// <summary>
    /// The dispute id
    /// </summary>
    public long DisputeId { get; set; } = -1;
}

public class CheckDisputeUpdateRequestToken
{
    /// <summary>
    /// The notice of dispute id.
    /// </summary>
    public Guid NoticeOfDisputeGuid { get; set; }

    /// <summary>
    /// The token encoded in the email used to validate the 
    /// email was received.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;
}

public class CheckDisputeUpdateRequestTokenResponse
{
    public CheckDisputeUpdateRequestTokenStatus Status { get; set; }
    public DateTimeOffset CheckedAt { get; set; }
}

public class DisputeUpdateRequestSuccessful
{
    public long DisputeId { get; set; }
    public DateTimeOffset AccessGrantedAt { get; set; }
}

/// <summary>
/// Indicates a new dispute has been submitted (created).
/// </summary>
public class DisputeUpdateRequestSubmitted
{
    public long DisputeId { get; set; }
 
    /// <summary>
    /// The notice of dispute id.
    /// </summary>
    public Guid NoticeOfDisputeGuid { get; set; }

    /// <summary>
    /// The token encoded in the email used to validate the 
    /// email was received.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;
}