using System.Threading;
using System.Threading.Tasks;
using DisputeApi.Web.Models;

namespace DisputeApi.Web.Features.TicketLookup
{
    public interface ITicketDisputeService
    {
        public Task<TicketDispute> RetrieveTicketDisputeAsync(string ticketNumber, string time, CancellationToken cancellationToken);
    }

    public abstract class TicketDisputeService : ITicketDisputeService
    {
        public async Task<TicketDispute> RetrieveTicketDisputeAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            var rawResponse =  await GetTicket(ticketNumber, time, cancellationToken);
            if (rawResponse?.Items == null || rawResponse.Items.Count == 0) return null;

            foreach (Item item in rawResponse.Items)
            {
                if (item.SelectedInvoice?.Reference == null) continue;
                var lastSlash = item.SelectedInvoice.Reference.LastIndexOf('/');
                if (lastSlash <= 0) continue;
                string invoiceNumber = item.SelectedInvoice.Reference.Substring(lastSlash + 1);
                Invoice invoice = await GetInvoice(invoiceNumber, cancellationToken);
                item.SelectedInvoice.Invoice = invoice;
            }

            return rawResponse.ConvertToTicketDispute();
        }

        protected abstract Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken);
        protected abstract Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken);
    }
}
