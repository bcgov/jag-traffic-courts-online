using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
