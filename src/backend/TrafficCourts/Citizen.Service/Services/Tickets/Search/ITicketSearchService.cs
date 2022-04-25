using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

/// <summary>
/// Provides an interface for searching for violation tickets.
/// </summary>
public interface ITicketSearchService
{
    /// <summary>
    /// Searches for a violation ticket with the matching ticket number and issue time.
    /// </summary>
    /// <param name="ticketNumber">The violation ticket number.</param>
    /// <param name="issuedTime">The time the violation ticket was issued at.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <exception cref="ArgumentNullException">if <paramref name="ticketNumber"/> is null.</exception>
    /// <exception cref="TicketSearchErrorException">An error occurred executing the search.</exception>
    /// <returns>The found <see cref="ViolationTicket"/> or null if the ticket was not found.</returns>
    Task<ViolationTicket?> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken);
}
