using MailKit.Net.Smtp;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface ISmtpClient
    {
        Task<MailKit.Net.Smtp.SmtpClient> CreateAsync(CancellationToken cancellationToken);
    }
}
