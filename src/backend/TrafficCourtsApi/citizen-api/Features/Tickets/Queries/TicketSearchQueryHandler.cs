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
using DBModelTicket = Gov.CitizenApi.Features.Tickets.DBModel.Ticket;
using Gov.CitizenApi.Features.Tickets.DBModel;
using System.Collections.Generic;

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
            TicketSearchResponse ticketSearchResponse=null;
            try
            {
                _logger.LogInformation("search ticket from rsi");
                ticketSearchResponse = await _ticketSearchClient.TicketsAsync(query.TicketNumber, query.Time, cancellationToken);
                _logger.LogInformation("ticket search from rsi completed successfully");
            }
            catch (TicketSearchException exception) when (exception.StatusCode == 204)
            {
                ticketSearchResponse = null;
            }

            if (ticketSearchResponse != null)
            {
                _logger.LogInformation("find the dispute for the ticket");
                Dispute dispute = await _disputeService.FindTicketDisputeAsync(ticketSearchResponse.ViolationTicketNumber);

                List<Payment> payments = _ticketService.FindTicketPayments(query.TicketNumber, query.Time);
                _logger.LogInformation("return ticketDispute.");
                return BuildTicketDispute(ticketSearchResponse, dispute, payments);
            }
            _logger.LogInformation("no ticket found from Rsi");

            //get ticket from DB, check if there is shell ticket created.
            //todo: following code is quite possible to change when we get to know where to save the shell ticket and if user can select ticket again.
            return await GetTicketDisputeFromShellTicket(query.TicketNumber, query.Time);
        }

        private async Task<TicketDispute> GetTicketDisputeFromShellTicket(string ticketNumber, string ticketTime)
        {
            _logger.LogInformation("find ticket from db");
            DBModelTicket ticket = await _ticketService.FindTicketAsync(ticketNumber, ticketTime);
            if (ticket != null)
            {
                _logger.LogInformation("find the dispute for the ticket");
                Dispute dispute = await _disputeService.FindTicketDisputeAsync(ticket.ViolationTicketNumber);
                _logger.LogInformation("return ticketDispute.");
                TicketSearchResponse response = _mapper.Map<TicketSearchResponse>(ticket);
                List<Payment> payments = _ticketService.FindTicketPayments(ticketNumber, ticketTime);
                TicketDispute ticketDispute = BuildTicketDispute(response, dispute, payments);
                //no dispute has been made, but frontend needs shell ticket person info as default disputant info
                //it is better to do it in front end.
                if (dispute == null)
                {
                    ticketDispute.Disputant = _mapper.Map<Disputant>(ticket);
                }
                return ticketDispute;
            }
            return null;
        }

        private TicketDispute BuildTicketDispute(TicketSearchResponse ticketSearchResponse, Dispute dispute, List<Payment> payments)
        {
            TicketDispute ticketDispute = _mapper.Map<TicketDispute>(ticketSearchResponse);
            if (dispute != null)
            {
                ticketDispute.Disputant = _mapper.Map<Disputant>(dispute);
                ticketDispute.Additional = _mapper.Map<Additional>(dispute);
            }
            foreach (Offence offence in ticketDispute.Offences)
            {
                var detail =
                    dispute?.OffenceDisputeDetails?.FirstOrDefault(m => m.OffenceNumber == offence.OffenceNumber);
                if (detail != null)
                {
                    offence.OffenceAgreementStatus=detail.OffenceAgreementStatus;
                    offence.RequestReduction = detail.RequestReduction;
                    offence.RequestMoreTime = detail.RequestMoreTime;
                    offence.ReductionAppearInCourt = detail.ReductionAppearInCourt;
                    offence.ReductionReason = detail.ReductionReason;
                    offence.MoreTimeReason = detail.MoreTimeReason;
                    offence.Status = detail.Status;
                }

                offence.AmountDue = (payments ==null || payments.Any(m => m.PaymentStatus==PaymentStatus.Success && m.RequestedCounts.Contains(offence.OffenceNumber.ToString()))) ? 0 : offence.AmountDue;

            }
            return ticketDispute;
        }

    }
}
