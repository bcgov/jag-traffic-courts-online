using System.Collections.Concurrent;

namespace TrafficCourts.Ticket.Search.Service.Features.Search.Rsi
{
    public class RoadSafetyTicketSearchService : ITicketSearchService
    {
        private readonly IRoadSafetyTicketSearchApi _api;
        private readonly ILogger<RoadSafetyTicketSearchService> _logger;

        public RoadSafetyTicketSearchService(IRoadSafetyTicketSearchApi api, ILogger<RoadSafetyTicketSearchService> logger)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<IEnumerable<Invoice>> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            var response = await _api.GetTicket(GetParameters(ticketNumber, time), cancellationToken);
            if (response.Items == null && !string.IsNullOrEmpty(response.Error))
            {
                // expected: "Traffic violation ticket not found. Traffic violation ticket numbers that begin with 'E' or 'S' are available to pay online. Please allow 48 hours for your ticket to appear. If you wish to pay now and your ticket is not available, please use one of the other methods of payments listed on the ticket"
                if (response.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Ticket not found, Road Safety service returned error message {Error}", response.Error);
                }
                else
                {
                    // not the error message we were expecting, log out at information level
                    _logger.LogInformation("Road Safety service returned error message {Error}", response.Error);
                }

                return Enumerable.Empty<Invoice>();
            }

            if (response != null && response.Items != null)
            {
                IEnumerable<Invoice> invoices = await GetInvoicesAsync(response.Items);
                return invoices;
            }

            return Enumerable.Empty<Invoice>();
        }

        private async Task<IEnumerable<Invoice>> GetInvoicesAsync(IEnumerable<Item> items)
        {
            if (items == null)
            {
                return Enumerable.Empty<Invoice>();
            }

            var invoices = new ConcurrentBag<Invoice>();

            await Parallel.ForEachAsync(items, async (item, cancellationToken) =>
            {
                if (item?.SelectedInvoice?.Reference is not null)
                {
                    // Reference will have this format: https://host/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/EZ020004602
                    string reference = item.SelectedInvoice.Reference;
                    var lastSlash = reference.LastIndexOf('/');
                    if (lastSlash != -1)
                    {
                        string invoiceNumber = reference[(lastSlash + 1)..];

                        var invoice = await _api.GetInvoice(invoiceNumber, cancellationToken);
                        invoices.Add(invoice);
                    }
                }
            });

            return invoices.OrderBy(_ => _.InvoiceNumber);
        }

        private static GetTicketParams GetParameters(string ticketNumber, string time)
        {
            return new GetTicketParams { TicketNumber = ticketNumber, Prn = "10006", IssuedTime = time.Replace(":", "") };
        }
    }

}
