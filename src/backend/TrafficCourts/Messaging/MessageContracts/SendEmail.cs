using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// Interface message contract for sending emails
    /// </summary>
    public class SendEmail : IMessage
    {
        public string From { get; set; } = String.Empty;
        public IList<string> To { get; set; } = new List<string>();
        public IList<string> Cc { get; set; } = new List<string>();
        public IList<string> Bcc { get; set; } = new List<string>();
        public string Subject { get; set; } = String.Empty;
        public string? PlainTextContent { get; set; }
        public string? HtmlContent { get; set; }
    }
}
