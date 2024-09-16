using System.Diagnostics;

namespace TrafficCourts.TicketSearch.Instrumentation;

public static class Diagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("Ticket.Search");
}
