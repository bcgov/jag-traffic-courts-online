using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using TrafficCourts.Ticket.Search.Service.Features.Search.RoadSafetyTicketSearch;

namespace TrafficCourts.Ticket.Search.Service.Features.Search.MockTicketSearch
{
    public class MockTicketSearchService : ITicketSearchService
    {
        public async Task<IEnumerable<Invoice>> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            List<Invoice> invoices = new List<Invoice>();

            for (int i = 1; i <= 3; i++)
            {
                var invoice = await GetInvoiceAsync($"{ticketNumber}{i}", cancellationToken);
                if (invoice is not null)
                {
                    invoices.Add(invoice);
                }
            }

            return invoices;
        }

        private async Task<Invoice> GetInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken)
        {
            return await DeserializeAsync<Invoice>($"Features/Search/MockTicketSearch/Data/invoice-{invoiceNumber}.json",
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
