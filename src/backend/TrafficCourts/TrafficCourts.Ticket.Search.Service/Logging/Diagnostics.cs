using System.Diagnostics;

namespace TrafficCourts.Ticket.Search.Service.Logging;

public class Diagnostics
{
    public const string ServiceName = "ticket-search";
    public static readonly ActivitySource Source = new ActivitySource(ServiceName);
}
