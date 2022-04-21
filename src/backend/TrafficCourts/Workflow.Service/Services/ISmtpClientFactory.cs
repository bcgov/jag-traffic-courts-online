using MailKit.Net.Smtp;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface ISmtpClientFactory
    {
        Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken);
    }
}
