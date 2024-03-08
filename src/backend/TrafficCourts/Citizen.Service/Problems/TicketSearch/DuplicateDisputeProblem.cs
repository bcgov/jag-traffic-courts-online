namespace TrafficCourts.Citizen.Service.Problems.TicketSearch;

/// <summary>
/// Bad Request
/// </summary>
public class DuplicateDisputeProblem : TicketSearchProblem
{
    public DuplicateDisputeProblem(string ticketNumber, string time, Guid errorId)
        : base(ticketNumber, time, "already-has-dispute", errorId)
    {
        Title = "A dispute can only be submitted once for a violation ticket.";
        Detail = $"A dispute has already been submitted for the ticket number {ticketNumber} issued at {time}.";
        Status = StatusCodes.Status400BadRequest;
    }
}
