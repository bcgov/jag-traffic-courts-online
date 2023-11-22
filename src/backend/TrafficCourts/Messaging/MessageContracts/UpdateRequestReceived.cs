namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Message to indicate a DisputeUpdateRequest was received
/// </summary>
public class UpdateRequestReceived
{
    /// <summary>
    /// The notice of dispute id.
    /// </summary>
    public Guid NoticeOfDisputeGuid { get; set; }
}
