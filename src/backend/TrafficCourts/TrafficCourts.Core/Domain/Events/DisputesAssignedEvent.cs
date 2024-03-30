using MediatR;
using System.Collections.Immutable;

namespace TrafficCourts.Domain.Events;

/// <summary>
/// Raised when one or more tickets are assigned to a user.
/// </summary>
/// <param name="ticketNumbers"></param>
/// <param name="username"></param>
public class DisputesAssignedEvent(IEnumerable<string> ticketNumbers, string? username) : INotification
{
    /// <summary>
    /// The ticket numbers assigned.
    /// </summary>
    public IReadOnlyCollection<string> TicketNumbers { get; } = ticketNumbers?.ToImmutableList() ?? Enumerable.Empty<string>().ToImmutableList();

    /// <summary>
    /// The username the tickets were assigned to.
    /// </summary>
    public string Username { get; } = username ?? string.Empty;
}