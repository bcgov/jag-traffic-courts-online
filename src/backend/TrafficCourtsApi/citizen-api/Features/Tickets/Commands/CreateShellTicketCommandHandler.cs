using AutoMapper;
using Gov.CitizenApi.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class CreateShellTicketCommand : ShellTicket, IRequest<CreateShellTicketResponse>
    {
    }
    public class CreateShellTicketResponse
    {
        public int Id { get; set; }
    }

    public class CreateShellTicketCommandHandler : IRequestHandler<CreateShellTicketCommand, CreateShellTicketResponse>
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;
        private readonly IMapper _mapper;

        public CreateShellTicketCommandHandler(ILogger<CreateShellTicketCommandHandler> logger, ITicketsService ticketService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CreateShellTicketResponse> Handle(CreateShellTicketCommand createTicket,
            CancellationToken cancellationToken)
        {
            var result = await _ticketsService.CreateTicketAsync(_mapper.Map<DBModel.Ticket>(createTicket));
            if (result.Id == 0)
            {
                _logger.LogInformation("Shell ticket already exists. ");
                return new CreateShellTicketResponse { Id = 0 };
            }
            else
            {
                _logger.LogInformation("Shell ticket is created. ");
                return new CreateShellTicketResponse { Id = result.Id };
            }
        }

    }
}