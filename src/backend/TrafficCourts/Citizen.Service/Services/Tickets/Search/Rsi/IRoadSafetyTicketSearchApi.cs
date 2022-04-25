using Refit;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi
{
    public interface IRoadSafetyTicketSearchApi
    {
        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/")]
        Task<RawTicketSearchResponse> GetTicket(GetTicketParams ticketParams, CancellationToken cancellationToken);

        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/{invoiceNumber}")]
        Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken);
    }
}
