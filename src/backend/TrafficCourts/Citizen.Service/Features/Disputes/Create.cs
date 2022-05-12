using MassTransit;
using MediatR;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Messaging.MessageContracts;
using AutoMapper;
using System.Diagnostics;
using NodaTime;

namespace TrafficCourts.Citizen.Service.Features.Disputes
{
    public static class Create
    {
        public class Request : IRequest<Response>
        {
            public NoticeOfDispute Dispute { get; init; }

            public Request(Models.Dispute.NoticeOfDispute dispute)
            {
                Dispute = dispute ?? throw new ArgumentNullException(nameof(dispute));
            }
        }
        public class Response
        {
            /// <summary>
            /// Creates a successful response.
            /// </summary>
            public Response()
            {
            }

            public Response(Exception exception)
            {
                Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            }

            public Exception? Exception { get; init; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ILogger _logger;
            private readonly IBus _bus;
            private readonly IRedisCacheService _redisCacheService;
            private readonly IFilePersistenceService _filePersistenceService;
            private readonly IMapper _mapper;
            private readonly IClock _clock;

            /// <summary>
            /// Creates the handler.
            /// </summary>
            /// <param name="bus"></param>
            /// <param name="redisCacheService"></param>
            /// <param name="mapper"></param>
            /// <param name="clock"></param>
            /// <param name="logger"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public Handler(IBus bus, IRedisCacheService redisCacheService, IFilePersistenceService filePersistenceService, IMapper mapper, IClock clock, ILogger<Handler> logger)
            {
                _bus = bus ?? throw new ArgumentNullException(nameof(bus));
                _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
                _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _clock = clock ?? throw new ArgumentNullException(nameof(clock));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                ArgumentNullException.ThrowIfNull(request);

                using Activity? activity = Diagnostics.Source.StartActivity("Create Dispute");

                NoticeOfDispute dispute = request.Dispute;
                string? ticketId = dispute.TicketId;
                string? ocrViolationTicketJson = null;
                OcrViolationTicket? violationTicket = null;
                Models.Tickets.ViolationTicket? lookedUpViolationTicket = null;
                MemoryStream? ticketImageStream = null;

                try
                {
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

                    if (ocrViolationTicketJson == null && lookedUpViolationTicket == null)
                    {
                        Exception ex = new ArgumentNullException("No associated Violation Ticket has been found");
                        _logger.LogError(ex, "Error creating dispute - No associated Violation Ticket has been found");
                        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        return new Response(ex);
                    }

                    SubmitNoticeOfDispute submitNoticeOfDispute = _mapper.Map<SubmitNoticeOfDispute>(dispute);
                    submitNoticeOfDispute.OcrViolationTicket = ocrViolationTicketJson;
                    if (lookedUpViolationTicket != null)
                    {
                        submitNoticeOfDispute.ViolationTicket = _mapper.Map<Messaging.MessageContracts.ViolationTicket>(lookedUpViolationTicket);
                    }
                    submitNoticeOfDispute.SubmittedDate = _clock.GetCurrentInstant().ToDateTimeUtc();

                    // Publish submit NoticeOfDispute event (consumer(s) will push event to Oracle Data API to save the Dispute and generate email)
                    await _bus.Publish(submitNoticeOfDispute, cancellationToken);

                    _logger.LogDebug("Dispute published to the queue for being consumed by consumers and saved in Oracle Data API");

                    // Send email message to the submitter's entered email - TODO: this needs to be moved to workflow service on SubmitNoticeOfDispute event
                    var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == "SubmitDisputeTemplate");
                    if (template is not null)
                    {
                        await _bus.Publish(new SendEmail()
                        {
                            From = template.Sender,
                            To = { submitNoticeOfDispute!.EmailAddress! },
                            Subject = template.SubjectTemplate.Replace("<ticketid>", dispute.TicketNumber),
                            PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", dispute.TicketNumber)
                        }, cancellationToken);
                    }

                    // success, return true
                    activity?.SetStatus(ActivityStatusCode.Ok);
                    return new Response();
                }
                catch (Exception exception)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
                    _logger.LogError(exception, "Error creating dispute");
                    return new Response(exception);
                }
            }
        }
    }
}
