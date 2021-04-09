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

    public class TicketDisputeFromRsiService : ITicketDisputeService
    {
        private readonly IRsiRestApi _rsiApi;
        public TicketDisputeFromRsiService(IRsiRestApi rsiApi)
        {
            _rsiApi = rsiApi;
        }

        public async Task<TicketDispute> RetrieveTicketDisputeAsync(string ticketNumber, string time)
        {
            RawTicketSearchResponse rawResponse = await _rsiApi.GetTicket(
                new GetTicketParams { TicketNumber = ticketNumber, PRN = "10006", IssuedTime = time.Replace(":", "") }
            );
            if (rawResponse == null || rawResponse.Items == null || rawResponse.Items.Count == 0) return null;

            foreach (Item item in rawResponse.Items)
            {
                if (item.SelectedInvoice?.Reference == null) continue;
                int lastSlash = item.SelectedInvoice.Reference.LastIndexOf('/');
                if (lastSlash <= 0) continue;
                string invoiceNumber = item.SelectedInvoice.Reference.Substring(lastSlash + 1);
                Invoice invoice = await _rsiApi.GetInvoice(invoiceNumber);
                item.SelectedInvoice.Invoice = invoice;
            }

            return rawResponse.ConvertToTicketDispute();
        }
    }

    public class TicketDisputeFromFilesService : ITicketDisputeService
    {
        public async Task<TicketDispute> RetrieveTicketDisputeAsync(string ticketNumber, string time)
        {
            await using FileStream openStream = File.OpenRead($"Features/TicketLookup/ticket-{ticketNumber}.json");
            RawTicketSearchResponse rawResponse =
                await JsonSerializer.DeserializeAsync<RawTicketSearchResponse>(openStream);

            if (rawResponse == null)
            {
                return null;
            }

            for (int i = 0; i < rawResponse.Items.Count; i++)
            {
                await using FileStream itemStream =
                    File.OpenRead($"Features/TicketLookup/invoice-{ticketNumber}{i + 1}.json");
                var invoice = await JsonSerializer.DeserializeAsync<Invoice>(itemStream);
                rawResponse.Items[i].SelectedInvoice.Invoice = invoice;
            }
            return rawResponse.ConvertToTicketDispute();
        }
    }
}
