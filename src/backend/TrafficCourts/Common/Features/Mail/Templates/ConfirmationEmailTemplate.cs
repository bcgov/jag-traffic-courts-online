namespace TrafficCourts.Common.Features.Mail.Templates;

public class ConfirmationEmailTemplate : MailTemplateCollectionEmailTemplate, IConfirmationEmailTemplate
{
    public ConfirmationEmailTemplate() : base("SubmitDisputeTemplate")
    {
    }
}
