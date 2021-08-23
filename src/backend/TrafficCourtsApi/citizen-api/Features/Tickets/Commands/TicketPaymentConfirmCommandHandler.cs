using AutoMapper;
using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.TicketSearch;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class TicketPaymentConfirmCommandHandler : IRequestHandler<TicketPaymentConfirmCommand, TicketPaymentConfirmResponse>
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;

        private readonly IMapper _mapper;

        public TicketPaymentConfirmCommandHandler(
            ITicketSearchClient ticketSearchClient, 
            ILogger<TicketPaymentConfirmCommandHandler> logger, 
            ITicketsService ticketService,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TicketPaymentConfirmResponse> Handle(TicketPaymentConfirmCommand ticketPaymentConfirmCommand,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("update ticket payment");
            Payment payment = await _ticketsService.UpdatePaymentAsync(_mapper.Map<DBModel.Payment>(ticketPaymentConfirmCommand));
            return new TicketPaymentConfirmResponse { TicketNumber=payment.ViolationTicketNumber, Time=payment.ViolationTime};
        }
    }
}