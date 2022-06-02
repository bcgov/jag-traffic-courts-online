using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.Features.Mail.Model;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for DisputeApproved message, which will send an e-mail notification
    ///     to the disputant, that the dispute was approved for processing.
    /// </summary>
    public class DisputeApprovedNotifyConsumer : IConsumer<DisputeApproved>
    {
        private readonly ILogger<DisputeApprovedNotifyConsumer> _logger;
        private static readonly string _approveEmailTemplateName = "ProcessingDisputeTemplate";

        public DisputeApprovedNotifyConsumer(ILogger<DisputeApprovedNotifyConsumer> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Consumer looks-up the e-mail template to use and generates an e-mail message
        /// based on the template for publishing
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<DisputeApproved> context)
        {
            // Send email message to the submitter's entered email to notify of dispute approval for processing
            var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == _approveEmailTemplateName);
            if (template is not null)
            {
                // TODO: there is future ability to opt-out of e-mails... may need to add check to skip over this, if disputant choses so.
                var emailMessage = new SendEmail()
                {
                    From = template.Sender,
                    To = { context.Message.Email! },
                    Subject = template.SubjectTemplate.Replace("<ticketid>", context.Message.TicketFileNumber),
                    PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", context.Message.TicketFileNumber)
                };

                await context.Publish(emailMessage);
            }
            else
            {
                _logger.LogError("Email {Template} not found", _approveEmailTemplateName);
            }
        }
    }
}
