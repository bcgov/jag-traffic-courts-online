using Gov.TicketSearch.Services.RsiServices.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Services
{
    public class FakeTicketsService : TicketsService
    {
        public FakeTicketsService(ILogger<TicketsService> logger) : base(logger)
        {
        }

        protected override async Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            return await DeserializeAsync<RawTicketSearchResponse>($"Services/FakeData/ticket-{ticketNumber}.json",
                cancellationToken: cancellationToken);
        }

        protected override async Task<Invoice> GetInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken)
        {
            return await DeserializeAsync<Invoice>($"Services/FakeData/invoice-{invoiceNumber}.json",
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
            T data = await JsonSerializer.DeserializeAsync<T>(fileStream, cancellationToken: cancellationToken);
            return data;
        }
    }
}
