namespace TrafficCourts.Workflow.Service.Sagas;

public class VerifyEmailAddressSagaState : BaseStateMachineState
{
    public Guid NoticeOfDisputeGuid { get; set; }

    public string EmailAddress { get; set; } = String.Empty;

    public string TicketNumber { get; set; } = String.Empty;

    /// <summary>
    /// The token to validate.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;

    /// <summary>
    /// Has the email address been verfied yet?
    /// </summary>
    public bool Verified { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }

    /// <summary>
    /// Have we been notified that the notice of dispute has been submitted?
    /// </summary>
    public long? DisputeId { get; set; }

    /// <summary>
    /// Is the message published for verification of a new email address as part of an update request.
    /// </summary>
    public bool IsUpdateEmailVerification { get; set; }
}
