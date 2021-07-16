using AutoMapper;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Models;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Tickets.Queries
{
    public class TicketSearchQueryHandler : IRequestHandler<TicketSearchQuery, TicketDispute>
    {
        private readonly ITicketsService _ticketDisputeService;
        private readonly IDisputeService _disputeService;
        private readonly IMapper _mapper;

        public TicketSearchQueryHandler(ITicketsService ticketDisputeService, IDisputeService disputeService,
            IMapper mapper)
        {
            _ticketDisputeService = ticketDisputeService;
            _disputeService = disputeService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TicketDispute> Handle(TicketSearchQuery query, CancellationToken cancellationToken)
        {
            //todo: change here to swagger generated httpClient to visit TicketSearch component.
            var ticketDispute = new TicketDispute();
            //todo
            if (ticketDispute != null)
            {
                Dispute dispute = await _disputeService.FindTicketDisputeAsync(ticketDispute.ViolationTicketNumber);
                MergeDisputeToTicketDispute(ticketDispute, dispute);
            }
            return ticketDispute;
        }
        private void MergeDisputeToTicketDispute(TicketDispute ticketDispute, Dispute dispute)
        {
            if (dispute == null) return;
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
        }
    }
}
