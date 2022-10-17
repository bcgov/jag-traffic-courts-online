using MassTransit;
using Microsoft.Extensions.Logging;
using NodaTime;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Sends a email message to a dispuant.
/// </summary>
public class SendEmailToDispuantConsumer : IConsumer<SendDispuantEmail>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly IClock _clock;
    private readonly ILogger<SendEmailToDispuantConsumer> _logger;

    public SendEmailToDispuantConsumer(IEmailSenderService emailSenderService, IClock clock, ILogger<SendEmailToDispuantConsumer> logger)
    {
        _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<SendDispuantEmail> context)
    {
        using var scope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeId);

        EmailMessage emailMessage = context.Message.Message;

        _logger.LogDebug("Calling email sender service");

        var result = await _emailSenderService.SendEmailAsync(emailMessage, context.CancellationToken);

        var now = _clock.GetCurrentInstant().ToDateTimeOffset();

        if (result == SendEmailResult.Success)
        {
            _logger.LogDebug("Sending email was successful");

            var message = new DispuantEmailSent
            {
                Message = emailMessage,
                TicketNumber = context.Message.TicketNumber,
                NoticeOfDisputeId = context.Message.NoticeOfDisputeId,
                SentAt = now
            };

            await context.PublishWithLog(_logger, message, context.CancellationToken);
        }
        else if (result == SendEmailResult.Filtered)
        {
            _logger.LogDebug("Sending email was filtered");

            var message = new DispuantEmailFiltered
            {
                Message = emailMessage,
                TicketNumber = context.Message.TicketNumber,
                NoticeOfDisputeId = context.Message.NoticeOfDisputeId,
                FilteredAt = now
            };

            await context.PublishWithLog(_logger, message, context.CancellationToken);
        }
    }
}
