using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// Interface message contract for sending emails
    /// </summary>
    public class SendEmail : IMessage
    {
        public string? FromEmailAddress { get; set; }
        public string? ToEmailAddress { get; set; } = String.Empty;
        public string Subject { get; set; } = String.Empty;
        public string? PlainTextContent { get; set; }
        public string? HtmlContent { get; set; }
        public string TicketNumber { get; set; } = String.Empty;
        public EmailHistorySuccessfullySent SuccessfullySent;
    }
}
