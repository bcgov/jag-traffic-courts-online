using Gov.TicketSearch.Services.RsiServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        protected ILogger<TicketsService> _logger;
        public TicketsService(ILogger<TicketsService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RawTicketSearchResponse> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            RawTicketSearchResponse rawResponse = await GetTicket(ticketNumber, time, cancellationToken);

            List<Item> items = rawResponse?.Items;
            if (items == null || items.Count == 0)
            {
                _logger.LogDebug("no items from RawTicketSearchResponse");
                return null;
            }

            await GetInvoicesAsync(items, cancellationToken);

            return rawResponse;
        }

        protected abstract Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken);
        protected abstract Task<Invoice> GetInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken);

        private async Task GetInvoicesAsync(List<Item> items, CancellationToken cancellationToken)
        {
            // fetch the invoices in parallel
            var getInvoiceTasks = GetInvoiceNumbers(items)
                .Select(invoice => Tuple.Create(invoice.Index, GetInvoiceAsync(invoice.InvoiceNumber, cancellationToken)))
                .ToList();

            // wait on all the invoice tasks
            await Task.WhenAll(getInvoiceTasks.Select(_ => _.Item2));

            foreach (var task in getInvoiceTasks)
            {
                items[task.Item1].SelectedInvoice.Invoice = await task.Item2;
            }
        }

        /// <summary>
        /// Gets the invoice numbers and the invoice number index.
        /// </summary>
        /// <returns>A sequence of tuples containing the invoice item index and invoice number.</returns>
        private static IEnumerable<(int Index, string InvoiceNumber)> GetInvoiceNumbers(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                string reference = item.SelectedInvoice?.Reference;
                if (reference == null) continue;

                // extract the invoice number from the last segment of the reference link
                var lastSlash = reference.LastIndexOf('/');
                if (lastSlash == -1) continue;

                string invoiceNumber = reference.Substring(lastSlash + 1);
                
                yield return (i, invoiceNumber);
            }
        }
    }
}