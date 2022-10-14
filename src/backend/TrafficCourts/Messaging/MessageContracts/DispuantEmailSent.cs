using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// Raised when a message was successfully sent to a disputant.
    /// </summary>
    public class DispuantEmailSent
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
        public Guid NoticeOfDisputeId { get; set; }

        /// <summary>
        /// The date and time the message was sent at.
        /// </summary>
        public DateTimeOffset SentAt { get; set; }
    }
}
