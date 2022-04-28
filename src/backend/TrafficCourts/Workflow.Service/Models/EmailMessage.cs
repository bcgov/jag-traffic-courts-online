namespace TrafficCourts.Workflow.Service.Models
{
    /// <summary>
    /// SendEmail JSON converted over to an object for validation.
    /// </summary>
    public class EmailMessage
    {
        public string From { get; set; } = null!;
        public IList<string> To { get; set; }
        public IList<string> Cc { get; set; }
        public IList<string> Bcc { get; set; }
        public string Subject { get; set; } = null!;
        public string PlainTextContent { get; set; } = null!;
        public string HtmlContent { get; set; } = null!;
    }
}
