namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Message to indicate a DisputeUpdateRequest was accepted (a citizen submitted a request to update their Disputant contact information, which staff has approved).
/// Consumers of this message are expected to:
/// 
/// TCVP-1975
/// - call oracle-data-api to patch the Dispute with the DisputeUpdateRequest changes.
/// - call oracle-data-api to update DisputeUpdateRequest status.
/// - send confirmation email indicating request was accepted
/// - populate file/email history records
/// </summary>
public class DisputeUpdateRequestAccepted
{
    public DisputeUpdateRequestAccepted(long updateRequestId)
    {
        UpdateRequestId = updateRequestId;
    }

    public long UpdateRequestId { get; set; }
}
