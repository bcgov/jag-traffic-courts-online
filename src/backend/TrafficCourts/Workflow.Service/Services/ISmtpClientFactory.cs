using MailKit.Net.Smtp;

namespace TrafficCourts.Workflow.Service.Services;

public interface ISmtpClientFactory
{
    /// <summary>
    /// Creates a connected <see cref="ISmtpClient"/>.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Connected <see cref="ISmtpClient"/></returns>
    /// <exception cref="SmtpConnectFailedException">
    /// Could not connect to SMTP server. See inner exception for more detail.
    /// </exception>
    Task<ISmtpClient> CreateAsync(CancellationToken cancellationToken);
}
