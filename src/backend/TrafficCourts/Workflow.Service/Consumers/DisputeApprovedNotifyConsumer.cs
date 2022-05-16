using MassTransit;
using System.Text.Json;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
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

        public DisputeApprovedNotifyConsumer(ILogger<DisputeApprovedNotifyConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<DisputeApproved> context)
        {
            try
            {

                // Send email message to the submitter's entered email - TODO: this needs to be moved to workflow service on SubmitNoticeOfDispute event
                var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == "ProcessingDisputeTemplate");
                if (template is not null)
                {
                    var emailMessage = new SendEmail()
                    {
                        From = template.Sender,
                        To = { context.Message.Email! },
                        Subject = template.SubjectTemplate.Replace("<ticketid>", context.Message.TicketFileNumber),
                        PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", context.Message.TicketFileNumber)
                    };

                    await context.Publish(emailMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                throw;
            }
        }
    }
}
