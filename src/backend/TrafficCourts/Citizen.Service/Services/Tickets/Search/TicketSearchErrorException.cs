namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

[Serializable]
public class TicketSearchErrorException : Exception
{
    public TicketSearchErrorException(string message, Exception inner) : base(message, inner) { }
    protected TicketSearchErrorException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}