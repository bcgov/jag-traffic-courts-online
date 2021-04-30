using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DisputeApi.Web.Features.TicketLookup
{
    public static class TicketLookup
    {
        public class Query : IRequest<TicketDispute>
        {
            [FromQuery(Name = "ticketNumber")]
            [Required]
            [RegularExpression("^[A-Z]{2}[0-9]{6,}$",
                ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
            public string TicketNumber { get; set; }

            [FromQuery(Name = "time")]
            [Required]
            [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$",
                ErrorMessage = "time must be properly formatted 24 hour clock")]
            public string Time { get; set; }
        }

        public class TicketDisputeHandler : IRequestHandler<Query, TicketDispute>
        {
            private readonly ITicketDisputeService _ticketDisputeService;
            private readonly IDisputeService _disputeService;
            private readonly IMapper _mapper;

            public TicketDisputeHandler(ITicketDisputeService ticketDisputeService, IDisputeService disputeService,
                IMapper mapper)
            {
                _ticketDisputeService = ticketDisputeService;
                _disputeService = disputeService;
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<TicketDispute> Handle(Query query, CancellationToken cancellationToken)
            {
                var ticketDispute =
                    await _ticketDisputeService.RetrieveTicketDisputeAsync(query.TicketNumber, query.Time,
                        cancellationToken);
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
                        dispute.OffenceDisputeDetails.FirstOrDefault(m => m.OffenceNumber == offence.OffenceNumber);
                    if (detail != null)
                    {
                        offence.OffenceDisputeDetail = _mapper.Map<Models.OffenceDisputeDetail>(detail);
                    }
                }
            }
        }
    }
}