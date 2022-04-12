using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public interface SendEmail: IMessage
    {
        string From { get; set; }
        string[] To { get; set; }
        string[] Cc { get; set; }
        string[] Bcc { get; set; }
        string Subject { get; set; }
        string PlainTextContent { get; set; }
        string HtmlContent { get; set; }
    }
}
