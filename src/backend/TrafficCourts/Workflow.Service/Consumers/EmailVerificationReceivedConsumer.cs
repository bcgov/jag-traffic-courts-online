using MassTransit;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.Features.Mail.Templates;
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
    private readonly IConfirmationEmailTemplate _confirmationEmailTemplate;

    public EmailVerificationReceivedConsumer(ILogger<EmailVerificationReceivedConsumer> logger, IOracleDataApiService oracleDataApiService, IConfirmationEmailTemplate confirmationEmailTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _confirmationEmailTemplate = confirmationEmailTemplate;
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
            SaveFileHistoryRecord fileHistoryRecord = new SaveFileHistoryRecord();
            fileHistoryRecord.TicketNumber = dispute.TicketNumber;
            fileHistoryRecord.Description = "Email verification complete.";
            await context.Publish(fileHistoryRecord, context.CancellationToken);

            // File History 
            fileHistoryRecord.Description = "Dispute submitted for staff review.";
            await context.Publish(fileHistoryRecord, context.CancellationToken);

            // TCVP-1529 Send NoticeOfDisputeConfirmationEmail *after* validating Disputant's email
            EmailMessage emailMessage = _confirmationEmailTemplate.Create(dispute);
            await context.Publish(new SendDispuantEmail
            {
                Message = emailMessage,
                TicketNumber = dispute.TicketNumber,
                NoticeOfDisputeId = Guid.Empty   // TODO: set correct NoticeOfDisputeId
            }, context.CancellationToken);

            dispute.EmailAddressVerified = true;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to validate Disputant email.");
            throw;
        }
    }
}
