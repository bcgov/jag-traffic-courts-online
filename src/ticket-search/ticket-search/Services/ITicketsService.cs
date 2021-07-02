using Gov.TicketSearch.Services.RsiServices.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Services
{
    public interface ITicketsService
    {
        public Task<RawTicketSearchResponse> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken);

    }

    public abstract class TicketsService : ITicketsService
    {
        public async Task<RawTicketSearchResponse> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            var rawResponse = await GetTicket(ticketNumber, time, cancellationToken);
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

            return rawResponse;
        }

        protected abstract Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken);
        protected abstract Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken);
    }
}