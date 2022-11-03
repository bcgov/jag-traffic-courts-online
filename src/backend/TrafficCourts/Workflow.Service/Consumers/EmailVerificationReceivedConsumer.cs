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
public class SetEmailVerifiedOnDisputeInDatabase : IConsumer<EmailVerificationSuccessful>
{
    private readonly ILogger<SetEmailVerifiedOnDisputeInDatabase> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IConfirmationEmailTemplate _confirmationEmailTemplate;

    public SetEmailVerifiedOnDisputeInDatabase(ILogger<SetEmailVerifiedOnDisputeInDatabase> logger, IOracleDataApiService oracleDataApiService, IConfirmationEmailTemplate confirmationEmailTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _confirmationEmailTemplate = confirmationEmailTemplate ?? throw new ArgumentNullException(nameof(confirmationEmailTemplate));
    }

    public async Task Consume(ConsumeContext<EmailVerificationSuccessful> context)
    {
        using var loggingScope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeGuid);

        var message = context.Message;
        try
        {
            Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(message.NoticeOfDisputeGuid, context.CancellationToken);
            if (dispute is null)
            {
                _logger.LogInformation("Dispute not found");
                return;
            }

            await _oracleDataApiService.VerifyDisputeEmailAsync(dispute.DisputeId, context.CancellationToken);

            // File History 
            SaveFileHistoryRecord fileHistoryRecord = new SaveFileHistoryRecord();
            fileHistoryRecord.TicketNumber = dispute.TicketNumber;
            fileHistoryRecord.Description = "Email verification complete";
            await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);

            // File History 
            fileHistoryRecord.Description = "Dispute submitted for staff review";
            await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);

            // TCVP-1529 Send NoticeOfDisputeConfirmationEmail *after* validating Disputant's email
            EmailMessage emailMessage = _confirmationEmailTemplate.Create(dispute);
            await context.PublishWithLog(_logger, new SendDispuantEmail
            {
                Message = emailMessage,
                TicketNumber = dispute.TicketNumber,
                NoticeOfDisputeGuid = Guid.Empty   // TODO: set correct NoticeOfDisputeGuid
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
