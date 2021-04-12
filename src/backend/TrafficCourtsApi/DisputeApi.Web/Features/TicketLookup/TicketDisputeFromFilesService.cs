using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace DisputeApi.Web.Features.TicketLookup
{
    public class TicketDisputeFromFilesService : TicketDisputeService
    {
        protected override async Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            return await DeserializeAsync<RawTicketSearchResponse>($"Features/TicketLookup/ticket-{ticketNumber}.json",
                cancellationToken: cancellationToken);
        }

        protected override async Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken)
        {
            return await DeserializeAsync<Invoice>($"Features/TicketLookup/invoice-{invoiceNumber}.json",
                cancellationToken: cancellationToken);
        }

        private async Task<T> DeserializeAsync<T>(string resourcePath, CancellationToken cancellationToken) where T : class
        {
            var provider = new ManifestEmbeddedFileProvider(GetType().Assembly);

            var fileInfo = provider.GetFileInfo(resourcePath);
            if (!fileInfo.Exists)
            {
                return null;
            }

            await using var fileStream = fileInfo.CreateReadStream();
            T data = await JsonSerializer.DeserializeAsync<T>(fileStream, cancellationToken:cancellationToken);
            return data;
        }
    }
}
