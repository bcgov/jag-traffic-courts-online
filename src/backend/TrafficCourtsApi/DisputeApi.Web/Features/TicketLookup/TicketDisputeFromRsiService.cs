using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace DisputeApi.Web.Features.TicketLookup
{
    public class TicketDisputeFromRsiService : TicketDisputeService
    {
        private readonly IRsiRestApi _rsiApi;
        public TicketDisputeFromRsiService(IRsiRestApi rsiApi)
        {
            _rsiApi = rsiApi;
        }

        protected override async Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            var result = await _rsiApi.GetTicket(
                new GetTicketParams {TicketNumber = ticketNumber, PRN = "10006", IssuedTime = time.Replace(":", "")},
                cancellationToken);
            return result;

        }

        protected override async Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken)
        {
            return await _rsiApi.GetInvoice(invoiceNumber: invoiceNumber, cancellationToken: cancellationToken);
        }
    }
}
