namespace TrafficCourts.Citizen.Service.Problems.TicketSearch;

/// <summary>
/// Internal Server Error: Returned when an exception was thrown when searching for a ticket. 
/// </summary>
public class TicketSearchFailedProblem : TicketSearchProblem
{
    public TicketSearchFailedProblem(string ticketNumber, string time, Guid errorId)
        : base(ticketNumber, time, "internal-server-error", errorId)
    {
        Title = "Error searching for ticket";
        Detail = $"There was an error searching for ticket number {ticketNumber} issued at {time}";
        Status = StatusCodes.Status500InternalServerError;
    }
}
