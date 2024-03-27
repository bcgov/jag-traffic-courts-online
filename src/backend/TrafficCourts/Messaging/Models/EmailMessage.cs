namespace TrafficCourts.Messaging.Models;

public class EmailMessage
{
    /// <summary>
    /// Who the email message is from.
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Who the email message is to.
    /// </summary>
    public string? To { get; set; }

    /// <summary>
    /// The subject of the email message
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The plain text conent of the email message.
    /// </summary>
    public string? TextContent { get; set; }

    /// <summary>
    /// The HTML conent of the email message.
    /// </summary>
    public string? HtmlContent { get; set; }
}
