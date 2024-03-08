namespace TrafficCourts.Citizen.Service.Problems.TicketSearch;

/// <summary>
/// Not Found: Returned when the specified ticket was not found. 
/// </summary>
public class TicketNotFoundProblem : TicketSearchProblem
{
    public TicketNotFoundProblem(string ticketNumber, string time)
        : base(ticketNumber, time, "not-found", Guid.Empty)
    {
        Title = "Ticket not found";
        Detail = $"Ticket number {ticketNumber} issued at {time} was not found";
        Status = StatusCodes.Status404NotFound;
    }
}
