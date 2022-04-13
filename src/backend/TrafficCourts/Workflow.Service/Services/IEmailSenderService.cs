using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(SendEmail emailMessage);
    }
}
