using MassTransit;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Sends a email message to a disputant.
/// </summary>
public class SendEmailToDisputantConsumer : IConsumer<SendDisputantEmail>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly TimeProvider _clock;
    private readonly ILogger<SendEmailToDisputantConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;

    public SendEmailToDisputantConsumer(IEmailSenderService emailSenderService, TimeProvider clock, ILogger<SendEmailToDisputantConsumer> logger, IOracleDataApiService oracleDataApiService)
    {
        _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService;
    }

    public async Task Consume(ConsumeContext<SendDisputantEmail> context)
    {
        using var scope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeGuid );
        _logger.LogDebug("Calling email sender service");

        try
        {
            // search for dispute log warning if not found
            Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(context.Message.NoticeOfDisputeGuid, context.CancellationToken);
            if (dispute is null) 
            {
                // avoid logging PII, do not log the message
                _logger.LogWarning("Sending email without existing dispute"); 
            }

            EmailMessage emailMessage = context.Message.Message;

            // try to fill in to if missing
            if (string.IsNullOrEmpty(emailMessage.To) && dispute is not null && dispute.EmailAddress is not null && dispute.EmailAddressVerified == true)
                emailMessage.To = dispute.EmailAddress;

            var result = string.IsNullOrEmpty(emailMessage.To) 
                ? SendEmailResult.Filtered 
                : await _emailSenderService.SendEmailAsync(emailMessage, context.CancellationToken);

            var now = _clock.GetUtcNow();

            // Do not log outgoing email if no dispute found since requires dispute id
            if (result == SendEmailResult.Success && dispute is not null)
            {
                _logger.LogDebug("Sending email was successful");

                var message = new DisputantEmailSent
                {
                    Message = emailMessage,
                    SentAt = now,
                    OccamDisputeId = dispute.DisputeId
                };

                await context.PublishWithLog(_logger, message, context.CancellationToken);
            }
            // Do not log outgoing email if no dispute found since requires dispute id
            else if (result == SendEmailResult.Filtered && dispute is not null)
            {
                _logger.LogDebug("Sending email was filtered");

                var message = new DisputantEmailFiltered
                {
                    Message = emailMessage,
                    FilteredAt = now,
                    OccamDisputeId = dispute.DisputeId
                };

                await context.PublishWithLog(_logger, message, context.CancellationToken);
            }
        }
        catch (ArgumentNullException ex) // log it and move on
        {
            // why does ArgumentNullException get thrown??
            _logger.LogError(ex, "Failed to send email");
        }
        catch (InvalidEmailMessageException ex) // log it and move on
        {
            _logger.LogError(ex, "Failed to send email, invalid email message");
        }
    }
}
