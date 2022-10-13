namespace TrafficCourts.Common.Features.Mail.Templates;

public class CancelledDisputeEmailTemplate : MailTemplateCollectionEmailTemplate, ICancelledDisputeEmailTemplate
{
    public CancelledDisputeEmailTemplate() : base("CancelledDisputeTemplate")
    {
    }
}
