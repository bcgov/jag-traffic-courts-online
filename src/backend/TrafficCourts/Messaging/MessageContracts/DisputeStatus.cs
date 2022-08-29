namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// An enumeration of available Statuses on a Dispute record.
    /// </summary>
    public enum DisputeStatus
    {
        /// <summary>
        /// The dispute is new.
        /// </summary>
        NEW,
        
        /// <summary>
        /// The dispute is being processed.
        /// </summary>
        PROCESSING,

        /// <summary>
        /// The dispute was rejected by the VTC.
        /// </summary>
        REJECTED,
        
        /// <summary>
        /// The dispute was cancelled by the disputant.
        /// </summary>
        CANCELLED,

        /// <summary>
        /// The dispute was validated by the VTC.
        /// </summary>
        VALIDATED
    }
}
