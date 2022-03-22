using MassTransit;
using MediatR;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Features.Disputes
{
    public static class Create
    {
        public class Request : TicketDispute, IRequest<Response>
        {

        }

        public class Response
        {
            public int Id { get; set; }
        }

        public class CreateDisputeHandler : IRequestHandler<Request, Response>
        {
            private readonly ILogger _logger;
            public readonly IRequestClient<SubmitDispute> _submitDisputeRequestClient;

            public CreateDisputeHandler(ILogger<CreateDisputeHandler> logger, IRequestClient<SubmitDispute> submitDisputeRequestClient)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _submitDisputeRequestClient = submitDisputeRequestClient ?? throw new ArgumentNullException(nameof(submitDisputeRequestClient));
            }

            public async Task<Response> Handle(Request createDisputeRequest, CancellationToken cancellationToken)
            {
                var response = await _submitDisputeRequestClient.GetResponse<DisputeSubmitted>(new
                {
                    Id = NewId.NextGuid(),
                    ticketNumber = createDisputeRequest.TicketNumber,
                    InVar.Timestamp
                });

                return new Response { Id = response.Message.DisputeId };
            }
        }
    }
}
