namespace TrafficCourts.Common.Features.Mail.Templates;

public class VerificationEmailTemplate : MailTemplateCollectionEmailTemplate, IVerificationEmailTemplate
{
    public VerificationEmailTemplate() : base("VerificationEmailTemplate")
    {
    }
}
