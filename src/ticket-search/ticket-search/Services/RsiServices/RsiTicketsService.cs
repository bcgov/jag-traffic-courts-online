using Gov.TicketSearch.Services.RsiServices.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Services.RsiServices
{
    public class RsiTicketsService : TicketsService
    {
        private readonly IRsiRestApi _rsiApi;
        public RsiTicketsService(IRsiRestApi rsiApi)
        {
            _rsiApi = rsiApi;
        }

        protected override async Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            var result = await _rsiApi.GetTicket(
                new GetTicketParams { TicketNumber = ticketNumber, Prn = "10006", IssuedTime = time.Replace(":", "") },
                cancellationToken);
            return result;

        }

        protected override async Task<Invoice> GetInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken)
        {
            return await _rsiApi.GetInvoice(invoiceNumber: invoiceNumber, cancellationToken: cancellationToken);
        }
    }
}
