using MassTransit;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Sends a email message to a disputant.
/// </summary>
public partial class SendEmailToDisputantConsumer : IConsumer<SendDisputantEmail>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly TimeProvider _clock;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly ILogger<SendEmailToDisputantConsumer> _logger;

    public SendEmailToDisputantConsumer(
        IEmailSenderService emailSenderService, 
        TimeProvider clock, 
        IOracleDataApiService oracleDataApiService, 
        ILogger<SendEmailToDisputantConsumer> logger)
    {
        _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
    }

    public async Task Consume(ConsumeContext<SendDisputantEmail> context)
    {
        CancellationToken cancellationToken = context.CancellationToken;

        using var scope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeGuid );

        Dispute? dispute = null;
        bool triedToGetDispute = false; // keep track if we tried to call GetDisputeAsync
        bool emailSent = false;

        try
        {
            // There is extra logic here to avoid calling into the Oracle Data API 
            // until absolutely nessassary. If the Oracle Data API is having a problem
            // we want to still send the email if possible. For example when submitting
            // your dispute, the email verification email should go out even if the 
            // Oracle Data API is having problems. Submitting notice of dispute will 
            // have the email address specified unless opted out. No "SendEmailToDisputantConsumer"
            // message would be populated in this situation.

            EmailMessage emailMessage = context.Message.Message;

            if (string.IsNullOrEmpty(emailMessage.To))
            {
                LogEmailToAddressMissing(); // really should always have a target email

                dispute = await GetDisputeAsync(context.Message.NoticeOfDisputeGuid, cancellationToken);
                if (dispute is null)
                {
                    LogEmailSentWithNoDispute();
                }
                else if (dispute.EmailAddress is not null && dispute.EmailAddressVerified)
                {
                    emailMessage.To = dispute.EmailAddress;
                }

                triedToGetDispute = true;
            }

            var result = await SendEmailAsync(emailMessage, cancellationToken);

            var now = _clock.GetUtcNow();

            // Do not log outgoing email if no dispute found since requires dispute id
            if (result == SendEmailResult.Success)
            {
                emailSent = true;
                LogEmailSent();

                if (!triedToGetDispute)
                {
                    dispute = await GetDisputeAsync(context.Message.NoticeOfDisputeGuid, cancellationToken);
                    triedToGetDispute = true;
                }

                if (dispute is not null)
                {
                    // publish that an email was sent to the target dispute id, if the dispute 
                    // does not yet exist, no history can be added
                    var message = new DisputantEmailSent
                    {
                        Message = emailMessage,
                        SentAt = now,
                        OccamDisputeId = dispute.DisputeId
                    };

                    await context.PublishWithLog(_logger, message, cancellationToken);
                }
            }
            // Do not log outgoing email if no dispute found since requires dispute id
            else if (result == SendEmailResult.Filtered)
            {
                LogEmailFiltered();

                if (!triedToGetDispute)
                {
                    dispute = await GetDisputeAsync(context.Message.NoticeOfDisputeGuid, cancellationToken);
                    triedToGetDispute = true;
                }

                if (dispute is not null)
                {
                    // publish that an email was filtered to the target dispute id, if the dispute 
                    // does not yet exist, no history can be added
                    var message = new DisputantEmailFiltered
                    {
                        Message = emailMessage,
                        FilteredAt = now,
                        OccamDisputeId = dispute.DisputeId
                    };

                    await context.PublishWithLog(_logger, message, context.CancellationToken);
                }
            }
        }
        catch (ArgumentNullException exception) // log it and move on
        {
            // why does ArgumentNullException get thrown??
            LogSendEmailFailed(exception);
        }
        catch (InvalidEmailMessageException exception) // log it and move on
        {
            LogInvalidEmailMessage(exception);
        }
        catch (Exception exception)
        {
            if (emailSent)
            {
                LogConsumeMessageFailedAfterEmailSent(exception);
            }
            else
            {
                LogConsumeMessageFailedWithoutEmailBeingSent(exception);
                throw;
            }
        }
    }

    private async Task<Dispute?> GetDisputeAsync(Guid noticeOfDisputeId, CancellationToken cancellationToken)
    {
        try
        {
            Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(noticeOfDisputeId, cancellationToken);
            return dispute;
        }
        catch (ApiException ex)
        {
            LogFailedToGetDisputeByNoticeOfDisputeGuid(ex);
            return null;
        }
    }

    private async Task<SendEmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(message.To))
        {
            return SendEmailResult.Filtered;
        }

        LogCallingEmailSenderService();
        var result = await _emailSenderService.SendEmailAsync(message, cancellationToken);
        return result;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Error occurred getting dispute by Notice NoticeOfDisputeId")]
    private partial void LogFailedToGetDisputeByNoticeOfDisputeGuid(ApiException exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Error occurred consuming message without email was sent, message will be requeued")]
    private partial void LogConsumeMessageFailedWithoutEmailBeingSent(Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Error occurred consuming message after email was sent, message will not be requeued")]
    private partial void LogConsumeMessageFailedAfterEmailSent(Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Calling email sender service")]
    private partial void LogCallingEmailSenderService();

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send email")]
    private partial void LogSendEmailFailed(Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Could not send email, invalid email message")]
    private partial void LogInvalidEmailMessage(Exception ex);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Sending email was filtered")]
    private partial void LogEmailFiltered();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Sending email was successful")]
    private partial void LogEmailSent();

    [LoggerMessage(Level = LogLevel.Warning, Message = "Sending email without existing dispute")]
    private partial void LogEmailSentWithNoDispute();

    [LoggerMessage(Level = LogLevel.Information, Message = "No To email address specified on email message")]
    private partial void LogEmailToAddressMissing();






}
