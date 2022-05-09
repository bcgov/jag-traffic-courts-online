using MediatR;
using OneOf;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
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
                throw new ArgumentException("ticketNumber must start with two upper case letters and 8 or more numbers", nameof(ticketNumber));
            }

            // use regex as well as TimeOnly.TryParse because we dont want seconds, milliseconds, etc.
            if (!Regex.IsMatch(time, TimeRegex))
            {
                throw new ArgumentException("time must be properly formatted 24 hour clock with only hours and minutes", nameof(time));
            }

            if (!TimeOnly.TryParse(time, out var timeOnly))
            {
                throw new ArgumentException("time must be properly formatted 24 hour clock with only hours and minutes", nameof(time));
            }

            TicketNumber = ticketNumber;
            Hour = timeOnly.Hour;
            Minute = timeOnly.Minute;
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
        private readonly IRedisCacheService _redisCacheService;

        public Handler(ITicketSearchService service, ILogger<Handler> logger, IRedisCacheService redisCacheService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
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

                activity?.SetStatus(ActivityStatusCode.Ok); // Ok means no Error, not that the search returned a value
                if (ticket is null)
                {
                    _logger.LogDebug("Not Found");
                    return Response.Empty;
                }

                using var replyScope = _logger.BeginScope(new Dictionary<string, object> { { "ViolationTicket", ticket } });
                _logger.LogTrace("Search complete");

                // Generate a guid for using as Violation Ticket Key to save looked up ticket data into Redis
                string ticketId = Guid.NewGuid().ToString("n");

                // Save the violation ticket data into Redis using the generated guid and set it to expire after 1 day from Redis 
                await _redisCacheService.SetRecordAsync<ViolationTicket>(ticketId, ticket, TimeSpan.FromDays(1));

                ticket.TicketId = ticketId;

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

