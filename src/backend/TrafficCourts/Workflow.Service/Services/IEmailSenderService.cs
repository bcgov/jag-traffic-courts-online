using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IEmailSenderService
    {
        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidEmailMessageException"></exception>
        /// <exception cref="EmailSendFailedException"></exception>
        Task<SendEmailResult> SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken);
    }
}
