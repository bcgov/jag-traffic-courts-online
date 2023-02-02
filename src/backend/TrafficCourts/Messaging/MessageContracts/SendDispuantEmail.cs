using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// Raised when an email should be sent to a dispuant.
    /// </summary>
    public class SendDispuantEmail
    {
        /// <summary>
        /// The email message.
        /// </summary>
        public EmailMessage Message { get; set; } = new();
        
        /// <summary>
        /// The ticket number.
        /// </summary>
        public string TicketNumber { get; set; } = String.Empty;

        /// <summary>
        /// The notice of dispute identifer.
        /// </summary>
        public Guid NoticeOfDisputeGuid { get; set; }
    }
}
