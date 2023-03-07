using MassTransit;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services.EmailTemplates;

namespace TrafficCourts.Workflow.Service.Consumers;

public class EmailVerificationSendConsumer : IConsumer<EmailVerificationSend>
{
    private readonly IVerificationEmailTemplate _emailTemplate;
    private readonly ILogger<EmailVerificationSendConsumer> _logger;

    public EmailVerificationSendConsumer(IVerificationEmailTemplate emailTemplate, ILogger<EmailVerificationSendConsumer> logger)
    {
        _emailTemplate = emailTemplate ?? throw new ArgumentNullException(nameof(emailTemplate));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<EmailVerificationSend> context)
    {
        using var scope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeGuid);

        _logger.LogDebug("Creating email reverification email");

        var emailMessage = _emailTemplate.Create(context.Message);

        var message = new SendDisputantEmail
        {
            Message = emailMessage,
            NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid,
            TicketNumber = context.Message.TicketNumber
        };

        await context.PublishWithLog(_logger, message, context.CancellationToken);
    }
}
