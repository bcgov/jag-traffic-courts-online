using System.Collections.Generic;

namespace TrafficCourts.Common.Contract
{

    [QueueName("NotificationRequested_queue")]
    public class NotificationContract
    {
        public NotificationMethod NotificationMethod { get; set; }
        public NotificationType NotificationType { get; set; }
        public TicketDisputeContract TicketDisputeContract { get; set; }
    }

    public enum NotificationMethod
    {
        Email,
        SMS
    }

    public enum NotificationType
    {
        Dispute
    }
}
