using System.Collections.Generic;

namespace TrafficCourts.Common.Contract
{

    [QueueName("NotificationRequested_queue")]
    public class NotificationContract
    {
        public string ViolationTicketNumber { get; set; }
        public NotificationType NotificationType { get; set; }

    }

    public enum NotificationType
    {
        Email,
        SMS
    }

}
