using MediatR;
using OneOf;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Logging;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services.Tickets.Search;

namespace TrafficCourts.Citizen.Service.Features.Tickets;

public static class Search
{
    public class Request : IRequest<Response>
    {
        public const string TicketNumberRegex = "^[A-Z]{2}[0-9]{8}$";
        public const string TimeRegex = "^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$";

        public string TicketNumber { get; }

        /// <summary>
        /// The 24 hour clock
        /// </summary>
        public int Hour { get; }
        public int Minute { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticketNumber"></param>
        /// <param name="time"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Request(string ticketNumber, string time)
        {
            ArgumentNullException.ThrowIfNull(ticketNumber);
            ArgumentNullException.ThrowIfNull(time);

            if (!Regex.IsMatch(ticketNumber, TicketNumberRegex))
            {
                throw new ArgumentException("ticketNumber must start with two upper case letters and 6 or more numbers", nameof(ticketNumber));
            }

            if (!Regex.IsMatch(time, TimeRegex))
            {
                throw new ArgumentException("time must be properly formatted 24 hour clock", nameof(time));
            }

            TicketNumber = ticketNumber;
            Hour = int.Parse(time[0..2]);
            Minute = int.Parse(time[3..5]);
        }
    }

    public class Response
    {
        private Response()
        {
        }

        public Response(ViolationTicket ticket)
        {
            Result = ticket ?? throw new ArgumentNullException(nameof(ticket));
        }

        public Response(Exception exception)
        {
            Result = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        /// <summary>
        /// The result value.
        /// </summary>
        public OneOf<ViolationTicket, Exception> Result { get; }

        /// <summary>
        /// Represents an empty result, ie not found.
        /// </summary>
        public static readonly Response Empty = new();
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ITicketSearchService _service;
        private readonly ILogger<Handler> _logger;

        public Handler(ITicketSearchService service, ILogger<Handler> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            using Activity? activity = Diagnostics.Source.StartActivity("Ticket Search");

            using IDisposable? requestScope = _logger.BeginScope(new Dictionary<string, object> { { "Request", request } });
            _logger.LogTrace("Begin handler");

            try
            {
                _logger.LogDebug("Searching for ticket");
                ViolationTicket? ticket = await _service.SearchAsync(request.TicketNumber, new TimeOnly(request.Hour, request.Minute), cancellationToken);
                if (ticket is null)
                {
                    _logger.LogDebug("Not Found");
                    return Response.Empty;
                }

                using var replyScope = _logger.BeginScope(new Dictionary<string, object> { { "ViolationTicket", ticket } });
                _logger.LogTrace("Search complete");

                activity?.SetStatus(ActivityStatusCode.Ok);                
                return new Response(ticket);
            }
            catch (Exception exception)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                _logger.LogError(exception, "Error searching for ticket");
                return new Response(exception);
            }
        }
    }
}

