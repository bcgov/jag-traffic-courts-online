using MassTransit;
using MediatR;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.Features.FilePersistence;
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
            private readonly IFilePersistenceService _filePersistenceService;
            private readonly IMapper _mapper;

            public CreateDisputeHandler(ILogger<CreateDisputeHandler> logger,
                IRequestClient<Messaging.MessageContracts.SubmitNoticeOfDispute> submitDisputeRequestClient,
                IRequestClient<SendEmail> sendEmailRequestClient,
                IRedisCacheService redisCacheService,
                IFilePersistenceService filePersistenceService,
                IMapper mapper)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _submitDisputeRequestClient = submitDisputeRequestClient ?? throw new ArgumentNullException(nameof(submitDisputeRequestClient));
                _sendEmailRequestClient = sendEmailRequestClient ?? throw new ArgumentNullException(nameof(sendEmailRequestClient));
                _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
                _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Models.Dispute.NoticeOfDispute createDisputeRequest = request.Dispute;
                string? ticketId = createDisputeRequest.TicketId;
                string? ocrViolationTicketJson = null;
                OcrViolationTicket? violationTicket = null;
                Models.Tickets.ViolationTicket? lookedUpViolationTicket = null;
                MemoryStream? ticketImageStream = null;

                // Check if the request contains ticket id and it's a valid format guid
                if (ticketId != null && Guid.TryParseExact(ticketId, "n", out _))
                {
                    // Get the OCR violation ticket data from Redis cache using the ticket id key
                    violationTicket = await _redisCacheService.GetRecordAsync<OcrViolationTicket>(ticketId);

                    // TODO: This check works for now to determine which type of object is returned from redis cache
                    // and assign null or populate the object in SubmitNoticeOfDispute message. However, a better check/method can be implemented
                    if (violationTicket != null && violationTicket.ImageFilename != null)
                    {
                        // Since the ImageFilename exists, it should be in the redis db.
                        // grab it and save to the file persistence service.
                        ticketImageStream = await _redisCacheService.GetFileRecordAsync(violationTicket.ImageFilename);

                        if (ticketImageStream is not null)
                        {
                            var filename = await _filePersistenceService.SaveFileAsync(ticketImageStream, cancellationToken);

                            // remove the image from the redis cache, to free-up the space.
                            await _redisCacheService.DeleteRecordAsync(violationTicket.ImageFilename);

                            // re-set the imagefilename, as it may have potentially changed.
                            violationTicket.ImageFilename = filename;
                        }

                        // Serialize OCR violation ticket to a JSON string
                        ocrViolationTicketJson = JsonSerializer.Serialize(violationTicket);
                    }
                    else
                    {
                        // Get the looked up violation ticket data from Redis cache using the ticket id key
                        lookedUpViolationTicket = await _redisCacheService.GetRecordAsync<Models.Tickets.ViolationTicket>(ticketId);
                    }
                }

                SubmitNoticeOfDispute submitNoticeOfDispute = _mapper.Map<SubmitNoticeOfDispute>(createDisputeRequest);
                submitNoticeOfDispute.OcrViolationTicket = ocrViolationTicketJson;
                if (lookedUpViolationTicket != null)
                {
                    submitNoticeOfDispute.ViolationTicket = _mapper.Map<Messaging.MessageContracts.ViolationTicket>(lookedUpViolationTicket);
                }
                submitNoticeOfDispute.SubmittedDate = DateTime.UtcNow;
                
                var response = await _submitDisputeRequestClient.GetResponse<DisputeSubmitted>(submitNoticeOfDispute);

                // Send email message to the submitter's entered email
                var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == "SubmitDisputeTemplate");
                if (template is not null)
                {
                    var emailMessage = new SendEmail()
                    {
                        From = template.Sender,
                        To = { createDisputeRequest.EmailAddress },
                        Subject = template.SubjectTemplate.Replace("<ticketid>", createDisputeRequest.TicketNumber),
                        PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", createDisputeRequest.TicketNumber)
                    };
                    _sendEmailRequestClient.Create(emailMessage, cancellationToken);
                }

                return new Response(response.Message.DisputeId);
            }
        }
    }
}
