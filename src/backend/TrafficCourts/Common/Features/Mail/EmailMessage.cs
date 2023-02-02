namespace TrafficCourts.Common.Features.Mail;

public class EmailMessage
{
    public string? From { get; set; }
    public string To { get; set; } = String.Empty;
    public string Subject { get; set; } = String.Empty;
    public string? TextContent { get; set; }
    public string? HtmlContent { get; set; }
}

public enum SendEmailResult
{
    /// <summary>
    /// Unknown type (undefined). Must be index 0.
    /// </summary>
    Unknown,
    Filtered,
    Success
}
