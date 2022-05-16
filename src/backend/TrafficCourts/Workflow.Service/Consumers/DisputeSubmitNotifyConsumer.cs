using AutoMapper;
using MassTransit;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;
using TrafficCourts.Workflow.Service.Services;
using ViolationTicketCount = TrafficCourts.Workflow.Service.Models.ViolationTicket;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SubmitNoticeOfDispute message, which will send an e-mail notification of submission,
    ///     to the submitter's supplied e-mail address.
    /// </summary>
    public class DisputeSubmitNotifyConsumer : IConsumer<SubmitNoticeOfDispute>
    {
        private readonly ILogger<DisputeSubmitNotifyConsumer> _logger;
        private readonly IMapper _mapper;

        public DisputeSubmitNotifyConsumer(ILogger<DisputeSubmitNotifyConsumer> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
        {
            try
            {
                NoticeOfDispute noticeOfDispute = _mapper.Map<NoticeOfDispute>(context.Message);

                // Send email message to the submitter's entered email - TODO: this needs to be moved to workflow service on SubmitNoticeOfDispute event
                var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == "SubmitDisputeTemplate");
                if (template is not null)
                {
                    var emailMessage = new SendEmail()
                    {
                        From = template.Sender,
                        To = { noticeOfDispute!.EmailAddress! },
                        Subject = template.SubjectTemplate.Replace("<ticketid>", noticeOfDispute.TicketNumber),
                        PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", noticeOfDispute.TicketNumber)
                    };

                    var emailSent = JsonSerializer.Serialize(emailMessage);
                    _logger.LogDebug(emailSent);

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
