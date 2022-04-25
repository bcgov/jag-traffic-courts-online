using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

/// <summary>
/// Search for raw ticket invoices.
/// </summary>
public interface ITicketInvoiceSearchService
{
    /// <summary>
    /// Searches for a violation ticket invoices (counts) with the matching ticket number and issue time.
    /// </summary>
    /// <param name="ticketNumber">The violation ticket number.</param>
    /// <param name="issuedTime">The time the violation ticket was issued at.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The matching violation ticket invoices (counts).</returns>
    Task<IEnumerable<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken);
}
