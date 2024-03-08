using Microsoft.AspNetCore.Mvc;

namespace TrafficCourts.Citizen.Service.Problems.TicketSearch;

public abstract class TicketSearchProblem : ProblemDetails
{
    protected TicketSearchProblem(string ticketNumber, string time, string type, Guid errorId)
    {
        Type = $"https://tickets.gov.bc.ca/problems/ticket-search/{type}";
        Ticket = new TicketSearchDetail(ticketNumber, time);
        ErrorId = errorId;
        Instance = $"https://tickets.gov.bc.ca/problems/ticket-search/{type}?errorId={ErrorId}";
    }

    /// <summary>
    /// The ticket number searched for.
    /// </summary>
    public TicketSearchDetail Ticket { get; }

    public Guid ErrorId { get; }
}
