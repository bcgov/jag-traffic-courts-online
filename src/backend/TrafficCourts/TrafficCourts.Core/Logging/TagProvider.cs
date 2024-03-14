using Microsoft.Extensions.Logging;

namespace TrafficCourts.Logging;

public static class TagProvider
{
    /// <summary>
    /// Logs a TicketNumber property if it is not null or empty. 
    /// </summary>
    public static void RecordTicketNumber(ITagCollector collector, string ticketNumber)
    {
        if (!string.IsNullOrEmpty(ticketNumber))
        {
            collector.Add("TicketNumber", ticketNumber);
        }
    }

    /// <summary>
    /// Logs a Username property if it is not null or empty. 
    /// </summary>
    /// <param name="collector"></param>
    /// <param name="username"></param>
    public static void RecordUsername(ITagCollector collector, string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            collector.Add("Username", username);
        }
    }
}
