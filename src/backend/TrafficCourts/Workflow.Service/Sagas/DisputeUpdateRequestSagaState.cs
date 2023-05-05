namespace TrafficCourts.Workflow.Service.Sagas;

public class DisputeUpdateRequestSagaState : BaseStateMachineState
{
    public Guid NoticeOfDisputeGuid { get; set; }

    public string TicketNumber { get; set; } = String.Empty;

    /// <summary>
    /// The token to validate.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;

    /// <summary>
    /// The BC Services Card Token
    /// </summary>
    public Guid BCSCToken { get; set; } = Guid.Empty;

    /// <summary>
    /// Has the access to save been granted yet?
    /// </summary>
    public bool DisputeUpdateRequestAccessGranted { get; set; }
    public DateTimeOffset? DisputeUpdateRequestAccessGrantedAt { get; set; }

    /// <summary>
    /// Have we been notified that the notice of dispute has been submitted?
    /// </summary>
    public long? DisputeId { get; set; }
}
