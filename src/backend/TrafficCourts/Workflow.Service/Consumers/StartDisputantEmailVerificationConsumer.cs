using MassTransit;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Starts the email verification process when a notice of dispute is submitted.
/// </summary>
public class StartDisputantEmailVerificationConsumer : IConsumer<SubmitNoticeOfDispute>
{
    private readonly ILogger<StartDisputantEmailVerificationConsumer> _logger;

    public StartDisputantEmailVerificationConsumer(ILogger<StartDisputantEmailVerificationConsumer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
    {
        using var loggingScope = _logger.BeginScope(context, message => message.NoticeOfDisputeId);

        Guid noticeOfDisputeId = context.Message.NoticeOfDisputeId;
        string emailAddress = context.Message.EmailAddress;
        string ticketNumber = context.Message.TicketNumber ?? string.Empty;

        bool optOutOfEmail = false; // TODO allow user opt out

        if (string.IsNullOrEmpty(emailAddress) || optOutOfEmail)
        {
            _logger.LogDebug("No email associated with dispute, will not send email verification");
            return;
        }

        if (optOutOfEmail)
        {
            _logger.LogDebug("Disputant has opted out of email communications, will not send email verification");
            return;
        }

        // TCVP-1529 Saving a dispute should send a verification email to the Disputant.
        var message = new RequestEmailVerification
        {
            NoticeOfDisputeId = noticeOfDisputeId,
            EmailAddress = emailAddress,
            TicketNumber = ticketNumber
        };

        await context.Publish(message, context.CancellationToken);
        _logger.PublishedMessage(message);
    }
}
