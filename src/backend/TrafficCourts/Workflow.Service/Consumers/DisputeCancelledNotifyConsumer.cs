using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for DisputeCancelled message, which will send an e-mail notification
    ///     to the disputant, that the dispute was approved for processing.
    /// </summary>
    public class DisputeCancelledNotifyConsumer : IConsumer<DisputeCancelled>
    {
        private readonly ILogger<DisputeCancelledNotifyConsumer> _logger;
        private static readonly string _emailTemplateName = "CancelledDisputeTemplate";

        public DisputeCancelledNotifyConsumer(ILogger<DisputeCancelledNotifyConsumer> logger)
        {
            _logger = logger;
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

            // Send email message to the submitter's entered email to notify of dispute approval for processing
            var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == _emailTemplateName);
            if (template is not null)
            {
                // TODO: there is future ability to opt-out of e-mails... may need to add check to skip over this, if disputant choses so.
                var emailMessage = new EmailMessage()
                {
                    From = template.Sender,
                    To = context.Message.Email!,
                    Subject = template.SubjectTemplate.Replace("<ticketid>", context.Message.TicketNumber),
                    TextContent = template.PlainContentTemplate?.Replace("<ticketid>", context.Message.TicketNumber),
                    HtmlContent = template.HtmlContentTemplate?.Replace("<ticketid>", context.Message.TicketNumber),
                };

                var sendDisputantEmail = new SendDisputantEmail()
                {
                    Message= emailMessage,
                    TicketNumber= context.Message.TicketNumber,
                    NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid
                };

                await context.PublishWithLog(_logger, sendDisputantEmail, context.CancellationToken);
            }
            else
            {
                _logger.LogError("Email {Template} not found", _emailTemplateName);
            }
        }
    }
}
