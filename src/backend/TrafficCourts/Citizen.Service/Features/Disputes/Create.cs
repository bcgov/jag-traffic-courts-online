using MassTransit;
using MediatR;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Messaging.MessageContracts;
using AutoMapper;

namespace TrafficCourts.Citizen.Service.Features.Disputes
{
    public static class Create
    {
        public class Request : IRequest<Response>
        {
            public Models.Dispute.NoticeOfDispute Dispute { get; init; }

            public Request(Models.Dispute.NoticeOfDispute dispute)
            {
                Dispute = dispute ?? throw new ArgumentNullException(nameof(dispute));
            }
        }

        public class Response
        {
            public Guid Id { get; init; }
            public Exception? Exception { get; init; }

            public Response(Guid id)
            {
                Id = id;
            }

            public Response(Exception exception)
            {
                Id = Guid.Empty;
                Exception = exception;
            }
        }

        public class CreateDisputeHandler : IRequestHandler<Request, Response>
        {
            private readonly ILogger _logger;
            public readonly IRequestClient<Messaging.MessageContracts.SubmitNoticeOfDispute> _submitDisputeRequestClient;
            public readonly IRequestClient<SendEmail> _sendEmailRequestClient;
            private readonly IRedisCacheService _redisCacheService;
            private readonly IMapper _mapper;

            public CreateDisputeHandler(ILogger<CreateDisputeHandler> logger, IRequestClient<Messaging.MessageContracts.SubmitNoticeOfDispute> submitDisputeRequestClient, IRequestClient<SendEmail> sendEmailRequestClient, IRedisCacheService redisCacheService, IMapper mapper)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _submitDisputeRequestClient = submitDisputeRequestClient ?? throw new ArgumentNullException(nameof(submitDisputeRequestClient));
                _sendEmailRequestClient = sendEmailRequestClient ?? throw new ArgumentNullException(nameof(sendEmailRequestClient));
                _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Models.Dispute.NoticeOfDispute createDisputeRequest = request.Dispute;
                string? ocrKey = createDisputeRequest.OcrKey;
                string? ocrViolationTicketJson = null;

                // Check if the request contains OCR key and it's a valid format guid
                if (ocrKey != null && Guid.TryParseExact(ocrKey, "n", out _))
                {
                    // Get the OCR violation ticket data from Redis cache using the OCR key and serialize it to a JSON string
                    OcrViolationTicket? violationTicket = await _redisCacheService.GetRecordAsync<OcrViolationTicket>(ocrKey);
                    if (violationTicket != null)
                    {
                        ocrViolationTicketJson = JsonSerializer.Serialize(violationTicket);
                    }
                }

                SubmitNoticeOfDispute submitNoticeOfDispute = _mapper.Map<SubmitNoticeOfDispute>(createDisputeRequest);
                submitNoticeOfDispute.ViolationTicket.OcrViolationTicket = ocrViolationTicketJson;
                submitNoticeOfDispute.CitizenSubmittedDate = DateTime.UtcNow;
                
                var response = await _submitDisputeRequestClient.GetResponse<DisputeSubmitted>(submitNoticeOfDispute);

                // Send email message to the submitter's entered email
                var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == "SubmitDisputeTemplate");
                if (template is not null)
                {
                    _sendEmailRequestClient.Create(new SendEmail()
                    {
                        From = template.Sender,
                        To = { createDisputeRequest.EmailAddress },
                        Subject = template.SubjectTemplate,
                        PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", createDisputeRequest.TicketNumber)
                    }, cancellationToken);
                }

                return new Response(response.Message.DisputeId);
            }
        }
    }
}
