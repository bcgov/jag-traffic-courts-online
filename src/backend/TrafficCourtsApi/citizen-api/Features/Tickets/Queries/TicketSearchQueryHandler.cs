using AutoMapper;
using Gov.CitizenApi.Features.Disputes;
using Gov.CitizenApi.Features.Disputes.DBModel;
using Gov.CitizenApi.Models;
using Gov.TicketSearch;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Offence = Gov.CitizenApi.Models.Offence;

namespace Gov.CitizenApi.Features.Tickets.Queries
{
    public class TicketSearchQueryHandler : IRequestHandler<TicketSearchQuery, TicketDispute>
    {
        private readonly ITicketSearchClient _ticketSearchClient;
        private readonly IDisputeService _disputeService;
        private readonly ITicketsService _ticketService;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketSearchQueryHandler> _logger;

        public TicketSearchQueryHandler(ITicketSearchClient ticketSearchClient, IDisputeService disputeService, IMapper mapper, ILogger<TicketSearchQueryHandler> logger, ITicketsService ticketsService)
        {
            _ticketSearchClient = ticketSearchClient ?? throw new ArgumentNullException(nameof(ticketSearchClient));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(ticketSearchClient));
            _ticketService = ticketsService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TicketDispute> Handle(TicketSearchQuery query, CancellationToken cancellationToken)
        {
            TicketSearchResponse ticketSearchResponse = await _ticketSearchClient.TicketsAsync(query.TicketNumber, query.Time, cancellationToken);
            _logger.LogInformation("ticket search from rsi completed successfully");
            if (ticketSearchResponse != null)
            {
                _logger.LogInformation("find the ticket from Rsi");
                Dispute dispute = await _disputeService.FindTicketDisputeAsync(ticketSearchResponse.ViolationTicketNumber);
                return BuildTicketDispute(ticketSearchResponse, dispute);
            }
            _logger.LogInformation("no ticket found from Rsi");

            //get ticket from DB, check if there is shell ticket created.
            //todo: following code is quite possible to change when we get to know where to save the shell ticket and if user can select ticket again.
            //Gov.CitizenApi.Features.Tickets.DBModel.Ticket ticket = await _ticketService.FindTicketAsync(query.TicketNumber);
            //if (ticket.ViolationTime == query.Time)
            //{ 
            //}
            return null;
            
        }
        private TicketDispute BuildTicketDispute(TicketSearchResponse ticketSearchResponse, Dispute dispute)
        {
            TicketDispute ticketDispute = _mapper.Map<TicketDispute>(ticketSearchResponse);
            if (dispute == null) return ticketDispute;
            ticketDispute.Disputant = _mapper.Map<Disputant>(dispute);
            ticketDispute.Additional = _mapper.Map<Additional>(dispute);
            ticketDispute.InformationCertified = dispute.InformationCertified;
            foreach (Offence offence in ticketDispute.Offences)
            {
                var detail =
                    dispute.OffenceDisputeDetails?.FirstOrDefault(m => m.OffenceNumber == offence.OffenceNumber);
                if (detail != null)
                {
                    offence.OffenceDisputeDetail = _mapper.Map<Models.OffenceDisputeDetail>(detail);
                }
            }
            return ticketDispute;
        }
    }
}
