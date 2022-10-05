using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using ApiException = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ApiException;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Consumer for a EmailReceivedVerification (produced when a Disputant confirms their email address).
/// This Consumer simply updates the Disputant record for the given email verification token, setting the EmailVerification flag to true.
/// </summary>
public class EmailVerificationReceivedConsumer : IConsumer<EmailVerificationReceived>
{
    private readonly ILogger<EmailVerificationReceivedConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IBus _bus;
    private readonly IEmailSenderService _emailSenderService;

    public EmailVerificationReceivedConsumer(ILogger<EmailVerificationReceivedConsumer> logger, IOracleDataApiService oracleDataApiService, IBus bus, IEmailSenderService emailSenderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(oracleDataApiService);
        _oracleDataApiService = (oracleDataApiService);
        ArgumentNullException.ThrowIfNull(bus);
        _bus = bus;
        ArgumentNullException.ThrowIfNull(emailSenderService);
        _emailSenderService = emailSenderService;
    }

    public async Task Consume(ConsumeContext<EmailVerificationReceived> context)
    {
        using var messageIdScope = _logger.BeginScope(new Dictionary<string, object> {
                { "MessageId", context.MessageId! },
                { "MessageType", nameof(EmailVerificationReceived) }
            });
        EmailVerificationReceived message = context.Message;
        try
        {
            string token = message.EmailVerificationToken.ToString();
            Dispute dispute = await _oracleDataApiService.GetDisputeByEmailVerificationTokenAsync(token);

            await _oracleDataApiService.VerifyDisputeEmailAsync(token);

            // File History 
            FileHistoryRecord fileHistoryRecord = new FileHistoryRecord();
            fileHistoryRecord.TicketNumber = dispute.TicketNumber;
            fileHistoryRecord.Description = "Email verification complete.";
            await context.Publish(fileHistoryRecord);

            // File History 
            fileHistoryRecord.Description = "Dispute submitted for staff review.";
            await context.Publish(fileHistoryRecord);

            // TCVP-1529 Send NoticeOfDisputeConfirmationEmail *after* validating Disputant's email
            SendEmail email = _emailSenderService.ToConfirmationEmail(dispute);
            await context.Publish(email);

            dispute.EmailAddressVerified = true;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to validate Disputant email.");
            throw;
        }
    }
}
