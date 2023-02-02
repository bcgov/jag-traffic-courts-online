using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Common.Features.Mail.Templates;

public abstract class MailTemplateCollectionEmailTemplate : IEmailTemplate<Dispute>
{
    private readonly string _templateName;

    public MailTemplateCollectionEmailTemplate(string templateName)
    {
        _templateName = templateName;
    }

    public EmailMessage Create(Dispute dispute)
    {
        var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == _templateName);
        if (template is null)
        {
            throw new EmailTemplateNameNotFoundException(_templateName);
        }

        EmailMessage emailMessage = new()
        {
            From = template.Sender,
            To = dispute.EmailAddress,
            Subject = template.SubjectTemplate.Replace("<ticketid>", dispute.TicketNumber),
            TextContent = template.PlainContentTemplate?.Replace("<ticketid>", dispute.TicketNumber),
            HtmlContent = template.HtmlContentTemplate?.Replace("<ticketid>", dispute.TicketNumber),
        };

        return emailMessage;
    }
}
