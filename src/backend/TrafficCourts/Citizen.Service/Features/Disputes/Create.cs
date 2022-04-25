using MassTransit;
using MediatR;
using System.Text.Json;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Features.Disputes
{
    public static class Create
    {
        public class Request : IRequest<Response>
        {
            public TicketDispute Dispute { get; init; }

            public Request(TicketDispute dispute)
            {
                Dispute = dispute ?? throw new ArgumentNullException(nameof(dispute));
            }
        }

        public class Response
        {
            public int Id { get; init; }
            public Exception? Exception { get; init; }

            public Response(int id)
            {
                Id = id;
            }

            public Response(Exception exception)
            {
                Id = -1;
                Exception = exception;
            }
        }

        public class CreateDisputeHandler : IRequestHandler<Request, Response>
        {
            private readonly ILogger _logger;
            public readonly IRequestClient<SubmitDispute> _submitDisputeRequestClient;
            private readonly IRedisCacheService _redisCacheService;

            public CreateDisputeHandler(ILogger<CreateDisputeHandler> logger, IRequestClient<SubmitDispute> submitDisputeRequestClient, IRedisCacheService redisCacheService)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _submitDisputeRequestClient = submitDisputeRequestClient ?? throw new ArgumentNullException(nameof(submitDisputeRequestClient));
                _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                TicketDispute createDisputeRequest = request.Dispute;
                string? ocrKey = createDisputeRequest.OcrKey;
                string? ocrViolationTicketJson = null;

                // Check if the request contains OCR key and it's a valid format guid
                if (ocrKey != null && Guid.TryParseExact(ocrKey, "n", out _))
                {
                    // Get the OCR violation ticket data from Redis cache using the OCR key and serialize it to a JSON string
                    OcrViolationTicket violationTicket = await _redisCacheService.GetRecordAsync<OcrViolationTicket>(ocrKey);
                    ocrViolationTicketJson = JsonSerializer.Serialize(violationTicket);
                }
                
                var response = await _submitDisputeRequestClient.GetResponse<DisputeSubmitted>(new
                {
                    TicketNumber = createDisputeRequest.TicketNumber,
                    CourtLocation = createDisputeRequest.CourtLocation,
                    ViolationDate = createDisputeRequest.ViolationDate,
                    GivenNames = createDisputeRequest.GivenNames,
                    DisputantSurname = createDisputeRequest.DisputantSurname,
                    StreetAddress = createDisputeRequest.StreetAddress,
                    Province = createDisputeRequest.Province,
                    PostalCode = createDisputeRequest.PostalCode,
                    HomePhone = createDisputeRequest.HomePhone,
                    DriversLicence = createDisputeRequest.DriversLicence,
                    DriversLicenceProvince = createDisputeRequest.DriversLicenceProvince,
                    WorkPhone = createDisputeRequest.WorkPhone,
                    DateOfBirth = createDisputeRequest.DateOfBirth,
                    EnforcementOrganization = createDisputeRequest.EnforcementOrganization,
                    ServiceDate = createDisputeRequest.ServiceDate,
                    TicketCounts = createDisputeRequest.TicketCounts,
                    LawyerRepresentation = createDisputeRequest.LawyerRepresentation,
                    InterpreterLanguage = createDisputeRequest.InterpreterLanguage,
                    WitnessIntent = createDisputeRequest.WitnessIntent,
                    OcrViolationTicket = ocrViolationTicketJson
                });

                return new Response(response.Message.DisputeId);
            }
        }
    }
}
