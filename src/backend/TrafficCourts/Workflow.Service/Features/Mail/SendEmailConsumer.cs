using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Features.Mail
{
    /// <summary>
    ///     Consumer for SendEmail message.
    /// </summary>
    public class SendEmailConsumer : IConsumer<SendEmail>
    {
        private readonly IEmailSenderService _emailSenderService;

        public SendEmailConsumer(IEmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
        }

        public async Task Consume(ConsumeContext<SendEmail> context)
        {
            await _emailSenderService.SendEmailAsync(context.Message, context.CancellationToken);
        }
    }
}
