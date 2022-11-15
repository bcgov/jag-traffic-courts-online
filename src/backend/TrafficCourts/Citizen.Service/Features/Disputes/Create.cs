using MassTransit;
using MediatR;
using System.Text.Json;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Messaging.MessageContracts;
using AutoMapper;
using System.Diagnostics;
using NodaTime;
using HashidsNet;
using TrafficCourts.Messaging;

namespace TrafficCourts.Citizen.Service.Features.Disputes
{
    public static class Create
    {
        public class Request : IRequest<Response>
        {
            public NoticeOfDispute Dispute { get; init; }

            public Request(NoticeOfDispute dispute)
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

            public Response(string noticeOfDisputeGuid)
            {
                NoticeOfDisputeGuid = EmailVerificationToken = noticeOfDisputeGuid;
            }

            public Exception? Exception { get; init; }
            public string? EmailVerificationToken { get; }
            public string? NoticeOfDisputeGuid { get;}
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ILogger _logger;
            private readonly IBus _bus;
            private readonly IRedisCacheService _redisCacheService;
            private readonly IFilePersistenceService _filePersistenceService;
            private readonly IMapper _mapper;
            private readonly IClock _clock;
            private readonly IHashids _hashids;

            /// <summary>
            /// Creates the handler.
            /// </summary>
            /// <param name="bus"></param>
            /// <param name="redisCacheService"></param>
            /// <param name="filePersistenceService"></param>
            /// <param name="mapper"></param>
            /// <param name="clock"></param>
            /// <param name="logger"></param>
            /// <param name="hashids"></param>
            /// <exception cref="ArgumentNullException"></exception>
            public Handler(IBus bus, IRedisCacheService redisCacheService, IFilePersistenceService filePersistenceService, IMapper mapper, IClock clock, ILogger<Handler> logger, IHashids hashids)
            {
                _bus = bus ?? throw new ArgumentNullException(nameof(bus));
                _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
                _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _clock = clock ?? throw new ArgumentNullException(nameof(clock));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
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
                    // Check if the request contains ticket id
                    if (!String.IsNullOrEmpty(ticketId))
                    {
                        // Check if the ticket id belongs to an OCR type of ticket
                        if (ticketId.EndsWith("-o"))
                        {
                            // Get the OCR violation ticket data from Redis cache using the ticket id key
                            violationTicket = await _redisCacheService.GetRecordAsync<OcrViolationTicket>(ticketId);

                            if (violationTicket is null || String.IsNullOrEmpty(violationTicket.ImageFilename))
                            {
                                _logger.LogInformation("No OCR violation ticket and image filename have been found for the {TicketId}", ticketId);
                                throw new TicketLookupFailedException($"No OCR violation ticket and image filename has been found for the {ticketId}");
                            }

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
                        // Check if the ticket id belongs to a Looked Up type of ticket
                        else if (ticketId.EndsWith("-l"))
                        {
                            // Get the looked up violation ticket data from Redis cache using the ticket id key
                            lookedUpViolationTicket = await _redisCacheService.GetRecordAsync<Models.Tickets.ViolationTicket>(ticketId);

                            if (lookedUpViolationTicket is null)
                            {
                                _logger.LogInformation("No looked up violation ticket has been found for the {TicketId}", ticketId);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("An invalid {TicketId} has been passed", ticketId);
                        }
                    }

                    if (ocrViolationTicketJson == null && lookedUpViolationTicket == null)
                    {
                        Exception ex = new TicketLookupFailedException("No associated Violation Ticket has been found");
                        _logger.LogError(ex, "Error creating dispute - No associated Violation Ticket has been found");
                        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        return new Response(ex);
                    }

                    SubmitNoticeOfDispute submitNoticeOfDispute = _mapper.Map<SubmitNoticeOfDispute>(dispute);
                    submitNoticeOfDispute.NoticeOfDisputeGuid = NewId.NextGuid();
                    submitNoticeOfDispute.OcrViolationTicket = ocrViolationTicketJson;
                    submitNoticeOfDispute.SubmittedTs = _clock.GetCurrentInstant().ToDateTimeUtc();

                    if (lookedUpViolationTicket != null)
                    {
                        submitNoticeOfDispute.ViolationTicket = _mapper.Map<Messaging.MessageContracts.ViolationTicket>(lookedUpViolationTicket);
                    }

                    // Publish submit NoticeOfDispute event (consumer(s) will push event to Oracle Data API to save the Dispute and generate email)
                    await _bus.PublishWithLog(_logger, submitNoticeOfDispute, cancellationToken);

                    // success, return true
                    activity?.SetStatus(ActivityStatusCode.Ok);

                    var hash = _hashids.EncodeHex(submitNoticeOfDispute.NoticeOfDisputeGuid.ToString("n"));
                    return new Response(hash);
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

    [Serializable]
    internal class TicketLookupFailedException : Exception
    {
        public TicketLookupFailedException(string? message) : base(message) { }
    }
}
