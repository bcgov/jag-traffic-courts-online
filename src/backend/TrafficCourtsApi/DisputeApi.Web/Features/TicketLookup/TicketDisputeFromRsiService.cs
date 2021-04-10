using System.Threading.Tasks;

namespace DisputeApi.Web.Features.TicketLookup
{
    public class TicketDisputeFromRsiService : TicketDisputeService
    {
        private readonly IRsiRestApi _rsiApi;
        public TicketDisputeFromRsiService(IRsiRestApi rsiApi)
        {
            _rsiApi = rsiApi;
        }

        protected override async Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time)
        {
            return await _rsiApi.GetTicket(
                new GetTicketParams { TicketNumber = ticketNumber, PRN = "10006", IssuedTime = time.Replace(":", "") }
            );
        }

        protected override async Task<Invoice> GetInvoice(string invoiceNumber)
        {
            return await _rsiApi.GetInvoice(invoiceNumber);
        }
    }
}
