using OneOf;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services.Tickets.Search;

namespace TrafficCourts.Citizen.Service.Features.Tickets.Search;

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

    public Response(InvalidTicketVersionException invalidTicketException)
    {
        Result = invalidTicketException ?? throw new ArgumentNullException(nameof(invalidTicketException));
    }

    /// <summary>
    /// The result value.
    /// </summary>
    public OneOf<ViolationTicket, Exception, InvalidTicketVersionException> Result { get; }

    /// <summary>
    /// Represents an empty result, ie not found.
    /// </summary>
    public static readonly Response Empty = new();
}


