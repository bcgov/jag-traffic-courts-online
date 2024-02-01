using MediatR;
using System.Text.RegularExpressions;

namespace TrafficCourts.Citizen.Service.Features.Tickets.Search;

public class Request : IRequest<Response>
{
    public const string TicketNumberRegex = "^[A-Z]{2}[0-9]{8}$";
    public const string TimeRegex = "^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$";

    public string TicketNumber { get; }

    /// <summary>
    /// The 24 hour clock
    /// </summary>
    public int Hour { get; }
    public int Minute { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticketNumber"></param>
    /// <param name="time"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Request(string ticketNumber, string time)
    {
        ArgumentNullException.ThrowIfNull(ticketNumber);
        ArgumentNullException.ThrowIfNull(time);

        if (!Regex.IsMatch(ticketNumber, TicketNumberRegex))
        {
            throw new ArgumentException("ticketNumber must start with two upper case letters and 8 or more numbers", nameof(ticketNumber));
        }

        // use regex as well as TimeOnly.TryParse because we dont want seconds, milliseconds, etc.
        if (!Regex.IsMatch(time, TimeRegex))
        {
            throw new ArgumentException("time must be properly formatted 24 hour clock with only hours and minutes", nameof(time));
        }

        if (!TimeOnly.TryParse(time, out var timeOnly))
        {
            throw new ArgumentException("time must be properly formatted 24 hour clock with only hours and minutes", nameof(time));
        }

        TicketNumber = ticketNumber;
        Hour = timeOnly.Hour;
        Minute = timeOnly.Minute;
    }
}


