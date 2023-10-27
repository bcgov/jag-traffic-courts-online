using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Workflow.Service.Sagas;

public class VerifyEmailAddressState : SagaStateMachineInstance
{
    [Key]
    public Guid CorrelationId { get; set; }
    
    [MaxLength(64)]
    public string CurrentState { get; set; }

    [Timestamp]
    public uint Version { get; set; }

    public Guid NoticeOfDisputeGuid => CorrelationId;

    [MaxLength(100)]
    public string EmailAddress { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// The token to validate.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;

    /// <summary>
    /// Has the email address been verfied yet?
    /// </summary>
    /// <remarks>Maps to occam_disputes.email_verified_yn</remarks>
    public bool Verified { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }

    /// <summary>
    /// Have we been notified that the notice of dispute has been submitted?
    /// </summary>
    /// <remarks>Maps to occam_disputes.dispute_id</remarks>
    public long? DisputeId { get; set; }

    /// <summary>
    /// Is the message published for verification of a new email address as part of an update request.
    /// </summary>
    public bool IsUpdateEmailVerification { get; set; }
}
