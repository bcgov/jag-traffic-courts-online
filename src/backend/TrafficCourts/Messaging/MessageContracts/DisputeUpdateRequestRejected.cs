namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Message to indicate a DisputeUpdateRequest was rejected (a citizen submitted a request to update their Disputant contact information, which staff has rejected).
/// Consumers of this message are expected to:
/// 
/// TCVP-1974
/// - call oracle-data-api to update DisputeUpdateRequest status.
/// - send confirmation email indicating request was rejected
/// - populate file/email history records
/// </summary>
public class DisputeUpdateRequestRejected
{
    public DisputeUpdateRequestRejected(long updateRequestId)
    {
        UpdateRequestId = updateRequestId;
    }

    public long UpdateRequestId { get; set; }
}
