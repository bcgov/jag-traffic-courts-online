using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TrafficCourts.Ticket.Search.Service.Features.Search
{
    public static class TicketSearch
    {
        public class Request : IRequest<Response>
        {
            /// <summary>
            /// The traffic violation ticket number
            /// </summary>
            [FromQuery(Name = "ticketNumber")]
            [Required]
            [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
            public string TicketNumber { get; set; }

            /// <summary>
            /// The time of traffic violation
            /// </summary>
            [FromQuery(Name = "time")]
            [Required]
            [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "time must be properly formatted 24 hour clock")]
            public string Time { get; set; }
        }

        public class Response
        {
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = null;

                return response;
            }
        }
    }
}
