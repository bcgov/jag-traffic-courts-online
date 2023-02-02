namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Message to indicate a DisputantUpdateRequest was accepted (a citizen submitted a request to update their Disputant contact information, which staff has approved).
/// Consumers of this message are expected to:
/// 
/// TCVP-1975
/// - call oracle-data-api to patch the Dispute with the DisputantUpdateRequest changes.
/// - call oracle-data-api to update DisputantUpdateRequest status.
/// - send confirmation email indicating request was accepted
/// - populate file/email history records
/// </summary>
public class DisputantUpdateRequestAccepted
{
    public DisputantUpdateRequestAccepted(long updateRequestId)
    {
        UpdateRequestId = updateRequestId;
    }

    public long UpdateRequestId { get; set; }
}
