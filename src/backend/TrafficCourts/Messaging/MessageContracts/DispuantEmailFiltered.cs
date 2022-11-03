using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// Raised when an email was filtered before being sent due to allow list processing.
    /// </summary>
    public class DispuantEmailFiltered
    {
        /// <summary>
        /// The email message.
        /// </summary>
        public EmailMessage Message { get; set; }

        /// <summary>
        /// The ticket number.
        /// </summary>
        public string TicketNumber { get; set; } = String.Empty;
        
        /// <summary>
        /// The notice of dispute identifer.
        /// </summary>
        public Guid NoticeOfDisputeGuid { get; set; }

        /// <summary>
        /// The date and time the message was processed and filtered.
        /// </summary>
        public DateTimeOffset FilteredAt { get; set; }
    }
}
