using MassTransit;
using Microsoft.Extensions.Logging;
using NodaTime;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Sends a email message to a disputant.
/// </summary>
public class SendEmailToDisputantConsumer : IConsumer<SendDisputantEmail>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly IClock _clock;
    private readonly ILogger<SendEmailToDisputantConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;

    public SendEmailToDisputantConsumer(IEmailSenderService emailSenderService, IClock clock, ILogger<SendEmailToDisputantConsumer> logger, IOracleDataApiService oracleDataApiService)
    {
        _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService;
    }

    public async Task Consume(ConsumeContext<SendDisputantEmail> context)
    {
        using var scope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeGuid );

        Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(NoticeOfDisputeGuid: context.Message.NoticeOfDisputeGuid, context.CancellationToken);

        EmailMessage emailMessage = context.Message.Message;

        _logger.LogDebug("Calling email sender service");

        var result = await _emailSenderService.SendEmailAsync(emailMessage, context.CancellationToken);

        var now = _clock.GetCurrentInstant().ToDateTimeOffset();

        if (result == SendEmailResult.Success)
        {
            _logger.LogDebug("Sending email was successful");

            var message = new DisputantEmailSent
            {
                Message = emailMessage,
                TicketNumber = context.Message.TicketNumber,
                NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid,
                SentAt = now,
                OccamDisputeId = dispute?.DisputeId is not null ? dispute.DisputeId : 0
            };

            if (dispute?.DisputeId is not null) await context.PublishWithLog(_logger, message, context.CancellationToken);
        }
        else if (result == SendEmailResult.Filtered)
        {
            _logger.LogDebug("Sending email was filtered");

            var message = new DisputantEmailFiltered
            {
                Message = emailMessage,
                TicketNumber = context.Message.TicketNumber,
                NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid,
                FilteredAt = now,
                OccamDisputeId = dispute?.DisputeId is not null ? dispute.DisputeId : 0
            };

           if (dispute?.DisputeId is not null) await context.PublishWithLog(_logger, message, context.CancellationToken);
        }
    }
}
