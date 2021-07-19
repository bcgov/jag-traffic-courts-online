using Gov.TicketSearch.Services.RsiServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Services.RsiServices
{
    public class RsiTicketsService : TicketsService
    {
        private readonly IRsiRestApi _rsiApi;
        public RsiTicketsService(IRsiRestApi rsiApi, ILogger<TicketsService> logger) :base(logger)
        {
            _rsiApi = rsiApi ?? throw new ArgumentNullException(nameof(rsiApi));
        }

        protected override async Task<RawTicketSearchResponse> GetTicket(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            _logger.LogDebug("send ticket search request to Rsi.");
            var result = await _rsiApi.GetTicket(
                new GetTicketParams { TicketNumber = ticketNumber, Prn = "10006", IssuedTime = time.Replace(":", "") },
                cancellationToken);
            _logger.LogDebug("get ticket info from Rsi : {@result}", result);
            return result;

        }

        protected override async Task<Invoice> GetInvoiceAsync(string invoiceNumber, CancellationToken cancellationToken)
        {
            var invoice = await _rsiApi.GetInvoice(invoiceNumber: invoiceNumber, cancellationToken: cancellationToken);
            _logger.LogDebug("get invoice from Rsi : {@invoice}", invoice);
            return invoice;
        }
    }
}
