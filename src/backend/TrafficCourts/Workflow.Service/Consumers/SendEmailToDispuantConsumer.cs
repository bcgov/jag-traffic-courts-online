using MassTransit;
using NodaTime;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
///     Consumer for SendEmail message.
/// </summary>
public class SendEmailToDispuantConsumer : IConsumer<SendDispuantEmail>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly IClock _clock;

    public SendEmailToDispuantConsumer(IEmailSenderService emailSenderService, IClock clock)
    {
        _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task Consume(ConsumeContext<SendDispuantEmail> context)
    {
        EmailMessage emailMessage = context.Message.Message;

        var result = await _emailSenderService.SendEmailAsync(emailMessage, context.CancellationToken);

        var now = _clock.GetCurrentInstant().ToDateTimeOffset();

        if (result == SendEmailResult.Success)
        {
            await context.Publish(new DispuantEmailSent
            {
                Message = emailMessage,
                TicketNumber = context.Message.TicketNumber,
                NoticeOfDisputeId = context.Message.NoticeOfDisputeId,
                SentAt = now
            }, context.CancellationToken);
        }
        else if (result == SendEmailResult.Filtered)
        {
            await context.Publish(new DispuantEmailFiltered
            {
                Message = emailMessage,
                TicketNumber = context.Message.TicketNumber,
                NoticeOfDisputeId = context.Message.NoticeOfDisputeId,
                FilteredAt = now
            }, context.CancellationToken);
        }
    }
}