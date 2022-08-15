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
        New,
        
        /// <summary>
        /// The dispute is being processed.
        /// </summary>
        Processing,

        /// <summary>
        /// The dispute was rejected by the VTC.
        /// </summary>
        Rejected,
        
        /// <summary>
        /// The dispute was cancelled by the disputant.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The dispute was validated by the VTC.
        /// </summary>
        Validated
    }
}
