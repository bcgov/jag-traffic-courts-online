using System;
using System.IO;
using System.Threading.Tasks;
using DisputeApi.Web.Models;
using System.Text.Json;

namespace DisputeApi.Web.Features.TicketLookup
{
    public interface ITicketDisputeService
    {
        public Task<TicketDispute> RetrieveTicketDisputeAsync(string ticketNumber, string time);
    }

    public abstract class TicketDisputeService : ITicketDisputeService
    {
        public async Task<TicketDispute> RetrieveTicketDisputeAsync(string ticketNumber, string time)
        {
            RawTicketSearchResponse rawResponse =  await GetTicket(ticketNumber, time);
            if (rawResponse == null || rawResponse.Items == null || rawResponse.Items.Count == 0) return null;

            foreach (Item item in rawResponse.Items)
            {
                if (item.SelectedInvoice?.Reference == null) continue;
                int lastSlash = item.SelectedInvoice.Reference.LastIndexOf('/');
                if (lastSlash <= 0) continue;
                string invoiceNumber = item.SelectedInvoice.Reference.Substring(lastSlash + 1);
                Invoice invoice = await GetInvoice(invoiceNumber);
                item.SelectedInvoice.Invoice = invoice;
            }

            return rawResponse.ConvertToTicketDispute();
        }

        protected abstract Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time);
        protected abstract Task<Invoice> GetInvoice(string invoiceNumber);
    }
}
