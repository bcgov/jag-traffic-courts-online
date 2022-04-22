using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

/// <summary>
/// Search for raw ticket invoices.
/// </summary>
public interface ITicketInvoiceSearchService
{
    Task<IEnumerable<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken);
}
