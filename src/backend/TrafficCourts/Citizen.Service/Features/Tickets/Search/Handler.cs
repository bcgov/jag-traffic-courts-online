using MediatR;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Services.Tickets.Search;

namespace TrafficCourts.Citizen.Service.Features.Tickets.Search;

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

            // Generate a guid with a suffix '-l' to indicate that it's a looked up ticketId
            // for using as Violation Ticket Key to save looked up ticket data into Redis
            string ticketId = $"{Guid.NewGuid():n}-l";

            // Save the violation ticket data into Redis using the generated guid and set it to expire after 1 day from Redis 
            await _redisCacheService.SetRecordAsync<ViolationTicket>(ticketId, ticket, TimeSpan.FromDays(1));

            ticket.TicketId = ticketId;

            return new Response(ticket);
        }
        catch (InvalidTicketVersionException exception)
        {
            _logger.LogError(exception, "Could not return a ticket with invalid violation date and version (VT1)");
            return new Response(exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error searching for ticket");
            return new Response(exception);
        }
    }
}


