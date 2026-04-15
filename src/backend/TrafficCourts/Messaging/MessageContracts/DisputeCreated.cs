namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// This event is issued after a dispute has been created in the database.
    /// </summary>
    public class DisputeCreated
    {
        /// <summary>
        /// The unique system generated notice of dispute identifer.
        /// </summary>
        public Guid NoticeOfDisputeGuid { get; set; }

        /// <summary>
        /// The violation ticket number.
        /// </summary>
        public string TicketNumber { get; set; } = string.Empty;

        /// <summary>
        /// The disputant's email address.
        /// </summary>
        public string? EmailAddress { get; set; }
    }
}
