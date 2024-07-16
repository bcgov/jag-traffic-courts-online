using Refit;
using TrafficCourts.TicketSearch.Common;

namespace TrafficCourts.TicketSearch.Rsi
{
    public interface IRoadSafetyTicketSearchApi
    {
        [Get("/api/v1/ticket/")]
        Task<RawTicketSearchResponse> GetTicket(GetTicketParams ticketParams, CancellationToken cancellationToken);

        [Get("/api/v1/ticket/{invoiceNumber}")]
        Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken);
    }
}
