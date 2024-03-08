namespace TrafficCourts.Citizen.Service.Problems.TicketSearch;

public class InvalidTicketVersionProblem : TicketSearchProblem
{
    public InvalidTicketVersionProblem(string ticketNumber, string time, Services.Tickets.Search.InvalidTicketVersionException exception)
        : base(ticketNumber, time, "wrong-version", exception.ErrorId)
    {
        Title = "Error searching for ticket";
        Detail = exception.Message;
        Status = StatusCodes.Status400BadRequest;
    }
}
