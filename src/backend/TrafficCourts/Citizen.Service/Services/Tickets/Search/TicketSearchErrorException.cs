using System.Runtime.Serialization;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

[Serializable]
public class TicketSearchErrorException : Exception
{
    public TicketSearchErrorException(string message, Exception? inner) : base(message, inner) { }
}
