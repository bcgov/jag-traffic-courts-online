using AutoMapper;
using Gov.CitizenApi.Messaging.Configuration;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class CreateShellTicketCommandHandler : IRequestHandler<CreateShellTicketCommand, CreateShellTicketResponse>
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly RabbitMQConfiguration _rabbitMqConfig;
        private readonly IMapper _mapper;

        public CreateShellTicketCommandHandler(ILogger<CreateShellTicketCommandHandler> logger, ITicketsService ticketService,
            ISendEndpointProvider sendEndpointProvider, IOptions<RabbitMQConfiguration> rabbitMqOptions, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _sendEndpointProvider =
                sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _rabbitMqConfig = rabbitMqOptions.Value ?? throw new ArgumentNullException(nameof(rabbitMqOptions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CreateShellTicketResponse> Handle(CreateShellTicketCommand createTicket,
            CancellationToken cancellationToken)
        {
            var result = await _ticketsService.CreateTicketAsync(_mapper.Map<DBModel.Ticket>(createTicket));
            if (result.Id == 0)
            {
                return new CreateShellTicketResponse { Id = 0 };
            }
            else
            {
                _logger.LogInformation("Shell ticket is created. ");
                //temp remove: todo: uncomment
                //await SendToQueue(_mapper.Map<DisputeContract>(result));
                //temp

                return new CreateShellTicketResponse { Id = result.Id };
            }
        }

        private async Task SendToQueue(DisputeContract dispute)
        {
            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri(
                    $"rabbitmq://{_rabbitMqConfig.Host}:{_rabbitMqConfig.Port}/{Constants.DisputeRequestedQueueName}"));

            await sendEndpoint.Send<DisputeContract>(dispute);
        }
    }
}