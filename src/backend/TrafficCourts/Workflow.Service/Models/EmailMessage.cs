
using System.Text;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Utils;

namespace TrafficCourts.Workflow.Service.Models
{
    public class EmailMessage
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
        public string[] Bcc { get; set; }
        public string Subject { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }

        public override string ToString()
        {
            return GetType().GetProperties()
                .Select(info => (info.Name, Value: info.GetValue(this, null) ?? "(null)"))
                .Aggregate(
                    new StringBuilder(),
                    (sb, pair) => sb.AppendLine($"{pair.Name}: {pair.Value}"),
                    sb => sb.ToString());
        }
    }
}
