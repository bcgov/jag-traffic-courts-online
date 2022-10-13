namespace TrafficCourts.Common.Features.Mail.Templates;

public class RejectedDisputeEmailTemplate : MailTemplateCollectionEmailTemplate, IRejectedDisputeEmailTemplate
{
    public RejectedDisputeEmailTemplate() : base("RejectedDisputeTemplate")
    {
    }
}
