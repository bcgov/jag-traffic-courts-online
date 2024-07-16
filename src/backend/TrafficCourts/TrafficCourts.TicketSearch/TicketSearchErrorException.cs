using System.Runtime.Serialization;

namespace TrafficCourts.TicketSearch;

[Serializable]
public class TicketSearchErrorException : Exception
{
    public TicketSearchErrorException(string message, Exception? inner) : base(message, inner) { }
}


[Serializable]
public class TicketSearchServiceUnavailableException : TicketSearchErrorException
{
    public TicketSearchServiceUnavailableException(string message, Exception? inner) : base(message, inner) { }
}