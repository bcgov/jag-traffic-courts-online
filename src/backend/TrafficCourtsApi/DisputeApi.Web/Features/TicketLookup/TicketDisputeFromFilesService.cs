using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.TicketLookup
{
    public class TicketDisputeFromFilesService : TicketDisputeService
    {
        protected override async Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time)
        {
            await using FileStream openStream = File.OpenRead($"Features/TicketLookup/ticket-{ticketNumber}.json");
            return await JsonSerializer.DeserializeAsync<RawTicketSearchResponse>(openStream);
        }

        protected override async Task<Invoice> GetInvoice(string invoiceNumber)
        {
            await using FileStream itemStream =
                File.OpenRead($"Features/TicketLookup/invoice-{invoiceNumber}.json");
            return await JsonSerializer.DeserializeAsync<Invoice>(itemStream);
        }
    }
}
