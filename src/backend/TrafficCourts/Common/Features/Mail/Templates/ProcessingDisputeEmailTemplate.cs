namespace TrafficCourts.Common.Features.Mail.Templates;

public class ProcessingDisputeEmailTemplate : MailTemplateCollectionEmailTemplate, IProcessingDisputeEmailTemplate
{
    public ProcessingDisputeEmailTemplate() : base("ProcessingDisputeTemplate")
    {
    }
}
