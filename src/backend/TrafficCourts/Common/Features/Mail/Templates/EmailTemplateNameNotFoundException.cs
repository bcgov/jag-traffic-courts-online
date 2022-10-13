namespace TrafficCourts.Common.Features.Mail.Templates;

public class EmailTemplateNameNotFoundException : Exception
{

    public EmailTemplateNameNotFoundException(string templateName) : base($"Email template {templateName} not found")
    {
        TemplateName = templateName;
    }

    public string TemplateName { get; init; }
}
