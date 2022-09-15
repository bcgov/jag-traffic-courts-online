using AutoMapper;
using MassTransit;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;

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
        private static readonly string _submitEmailTemplateName = "SubmitDisputeTemplate";

        public DisputeSubmitNotifyConsumer(ILogger<DisputeSubmitNotifyConsumer> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Consumer looks-up the e-mail template to use and generates an e-mail message
        /// based on the template for publishing
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
        {
            Dispute dispute = _mapper.Map<Dispute>(context.Message);

            // Send email message to the submitter's entered email
            var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == _submitEmailTemplateName);
            if (template is not null)
            {
                // TODO: there is future ability to opt-out of e-mails... may need to add check to skip over this, if disputant choses so.
                var emailMessage = new SendEmail()
                {
                    From = template.Sender,
                    To = { dispute!.EmailAddress! },
                    Subject = template.SubjectTemplate.Replace("<ticketid>", dispute.TicketNumber),
                    PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", dispute.TicketNumber),
                    TicketNumber = dispute.TicketNumber,
                };

                await context.Publish(emailMessage);
            }
            else
            {
                _logger.LogError("Email {Template} not found", _submitEmailTemplateName);
            }
        }
    }
}
