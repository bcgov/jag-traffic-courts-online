using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(SendEmail emailMessage, CancellationToken cancellationToken);
        SendEmail ToVerificationEmail(Dispute dispute);
        SendEmail ToConfirmationEmail(Dispute dispute);
    }
}
