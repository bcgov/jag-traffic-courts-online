namespace TrafficCourts.Workflow.Service.Models
{
    /// <summary>
    /// SendEmail JSON converted over to an object for validation.
    /// </summary>
    public class EmailMessage
    {
        public string From { get; set; } = null!;
        public string[] To { get; set; } = null!;
        public string[] Cc { get; set; } = null!;
        public string[] Bcc { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string PlainTextContent { get; set; } = null!;
        public string HtmlContent { get; set; } = null!;
    }
}
