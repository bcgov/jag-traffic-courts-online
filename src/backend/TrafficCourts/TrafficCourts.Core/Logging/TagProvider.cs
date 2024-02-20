using Microsoft.Extensions.Logging;

namespace TrafficCourts.Logging;

public static class TagProvider
{
    public static void RecordTicketNumber(ITagCollector collector, string ticketNumber)
    {
        collector.Add("TicketNumber", ticketNumber ?? string.Empty);
    }

    public static void RecordUsername(ITagCollector collector, string username)
    {
        collector.Add("Username", username ?? string.Empty);
    }
}
