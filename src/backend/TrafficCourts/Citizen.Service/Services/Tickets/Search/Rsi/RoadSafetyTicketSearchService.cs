using System.Collections.Concurrent;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi
{
    public class RoadSafetyTicketSearchService : ITicketInvoiceSearchService
    {
        private readonly IRoadSafetyTicketSearchApi _api;
        private readonly ILogger<RoadSafetyTicketSearchService> _logger;

        public RoadSafetyTicketSearchService(IRoadSafetyTicketSearchApi api, ILogger<RoadSafetyTicketSearchService> logger)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<IList<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            using var activity = Diagnostics.Source.StartActivity("RSI Ticket Search");

            RawTicketSearchResponse? response = null;

            try
            {
                var parameters = new GetTicketParams 
                { 
                    TicketNumber = ticketNumber, 
                    Prn = "10006", 
                    IssuedTime = $"{issuedTime.Hour:d2}{issuedTime.Minute:d2}"
                };

                response = await _api.GetTicket(parameters, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                var innerException = ex.InnerException;
                _logger.LogError(ex, "Error searching for RSI ticket");
                throw new TicketSearchErrorException("Error searching for RSI ticket", ex);
            }

            if (response.Items is null && !string.IsNullOrEmpty(response.Error))
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

                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                return Array.Empty<Invoice>();
            }

            if (response is not null && response.Items is not null)
            {
                IEnumerable<Invoice> invoices = await GetInvoicesAsync(response.Items);
                return invoices.ToList();
            }

            return Array.Empty<Invoice>();
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
                    if (reference.Length != 0)
                    {
                        string invoiceNumber = reference.Substring(reference.Length - 1);
                        var invoice = await _api.GetInvoice(invoiceNumber, cancellationToken);
                        invoices.Add(invoice);
                    }
                }
            });

            return invoices.OrderBy(_ => _.InvoiceNumber);
        }
    }

}
