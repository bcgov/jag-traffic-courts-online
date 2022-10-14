using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public abstract class EmailTemplate<T> : IEmailTemplate<T>
{
    protected string Sender = "DoNotReply@tickets.gov.bc.ca";

    public abstract EmailMessage Create(T data);
}
