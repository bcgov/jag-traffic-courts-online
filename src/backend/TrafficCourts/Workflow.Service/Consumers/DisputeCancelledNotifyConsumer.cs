using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services.EmailTemplates;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
///     Consumer for DisputeCancelled message, which will send an e-mail notification
///     to the disputant, that the dispute was approved for processing.
/// </summary>
public class DisputeCancelledNotifyConsumer : IConsumer<DisputeCancelled>
{
    private readonly ICancelledDisputeEmailTemplate _emailTemplate;
    private readonly ILogger<DisputeCancelledNotifyConsumer> _logger;

    public DisputeCancelledNotifyConsumer(
        ICancelledDisputeEmailTemplate emailTemplate,
        ILogger<DisputeCancelledNotifyConsumer> logger)
    {
        _emailTemplate = emailTemplate ?? throw new ArgumentNullException(nameof(emailTemplate));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Consumer looks-up the e-mail template to use and generates an e-mail message
    /// based on the template for publishing
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<DisputeCancelled> context)
    {
        using var scope = _logger.BeginConsumeScope(context);

        SendDisputantEmail message = new()
        {
            Message = _emailTemplate.Create(context.Message),
            TicketNumber = context.Message.TicketNumber,
            NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid
        };

        await context.PublishWithLog(_logger, message, context.CancellationToken);
    }
}
