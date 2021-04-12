using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using DisputeApi.Web.Features.Disputes;
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
            [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
            public string TicketNumber { get; set; }

            [FromQuery(Name = "time")]
            [Required]
            [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "time must be properly formatted 24 hour clock")]
            public string Time { get; set; }
        }

 

        public class TicketDisputeHandler : IRequestHandler<Query, TicketDispute>
        {
            private readonly ITicketDisputeService _ticketDisputeService;
            private readonly IDisputeService _disputeService;
            public TicketDisputeHandler(ITicketDisputeService ticketDisputeService, IDisputeService disputeService )
            {
                _ticketDisputeService = ticketDisputeService;
                _disputeService = disputeService;
            }

            public async Task<TicketDispute> Handle(Query query, CancellationToken cancellationToken)
            {
                var ticketDispute =
                    await _ticketDisputeService.RetrieveTicketDisputeAsync(query.TicketNumber, query.Time,
                        cancellationToken);
                if (ticketDispute != null)
                {
                    foreach (var offense in ticketDispute.Offenses)
                    {
                        offense.Dispute = await
                            _disputeService.FindDispute(ticketDispute.ViolationTicketNumber, offense.OffenseNumber);
                    }
                }

                return ticketDispute;
            }

        }

    }
}
