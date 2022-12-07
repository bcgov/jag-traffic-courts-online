namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Message to indicate a DisputantUpdateRequest was rejected (a citizen submitted a request to update their Disputant contact information, which staff has rejected).
/// Consumers of this message are expected to:
/// 
/// TCVP-1974
/// - call oracle-data-api to update DisputantUpdateRequest status.
/// - send confirmation email indicating request was rejected
/// - populate file/email history records
/// </summary>
public class DisputantUpdateRequestRejected
{
    public DisputantUpdateRequestRejected(long updateRequestId)
    {
        UpdateRequestId = updateRequestId;
    }

    public long UpdateRequestId { get; set; }
}
