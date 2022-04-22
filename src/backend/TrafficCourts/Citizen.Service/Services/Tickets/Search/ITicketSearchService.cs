using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

public interface ITicketSearchService
{
    Task<ViolationTicket?> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken);
}


[Serializable]
public class TicketSearchErrorException : Exception
{
    public TicketSearchErrorException(string message, Exception inner) : base(message, inner) { }
    protected TicketSearchErrorException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}