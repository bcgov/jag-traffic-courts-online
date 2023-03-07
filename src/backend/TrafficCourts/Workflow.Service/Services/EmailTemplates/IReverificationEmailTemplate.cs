using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public interface IReverificationEmailTemplate : IEmailTemplate<EmailVerificationSend>
{
}
