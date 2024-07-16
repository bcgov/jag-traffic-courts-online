using Microsoft.Extensions.Logging;
using TrafficCourts.TicketSearch.Common;

namespace TrafficCourts.TicketSearch.Rsi
{
    public class RoadSafetyTicketSearchService : ITicketInvoiceSearchService
    {
        private readonly IRoadSafetyTicketSearchApi _api;
        private readonly ILogger<RoadSafetyTicketSearchService> _logger;

        public RoadSafetyTicketSearchService(
            IRoadSafetyTicketSearchApi api,
            ILogger<RoadSafetyTicketSearchService> logger)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IList<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            (List<Invoice> invoices, bool isError) = await SearchRsiAsync(ticketNumber, issuedTime, cancellationToken).ConfigureAwait(false);
            return invoices;
        }

        private async Task<(List<Invoice> invoices, bool isError)> SearchRsiAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            using System.Diagnostics.Activity? activity = Instrumentation.Diagnostics.Source.StartActivity("rsi ticket search");
            activity?.AddTag("ticket.number", ticketNumber);
            activity?.AddTag("ticket.time", $"{issuedTime.Hour:D2}:{issuedTime.Minute:D2}");

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
            catch (Refit.ApiException exception) when (exception.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                // this is usually when the service is down for maintenance
                // TODO: add 
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                _logger.LogError(exception, "Error searching for RSI ticket, service is unavailable");
                throw new TicketSearchServiceUnavailableException("Error searching for RSI ticket", exception);
            }
            catch (HttpRequestException exception)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                var innerException = exception.InnerException;
                _logger.LogError(exception, "Error searching for RSI ticket");
                throw new TicketSearchErrorException("Error searching for RSI ticket", exception);
            }

            if (response.Items is null && !string.IsNullOrEmpty(response.Error))
            {
                return ProcessErrorResponse(activity, response);
            }

            if (response is not null && response.Items is not null)
            {
                var invoices = await ProcessItemResponse(activity, response, cancellationToken).ConfigureAwait(false);
                return invoices;
            }

            _logger.LogInformation("No invoice numbers returned, returning empty result");
            activity?.AddTag("rsi.response", "not found");
            return ([], false);
        }

        private async Task<(List<Invoice> invoices, bool isError)> ProcessItemResponse(System.Diagnostics.Activity? activity, RawTicketSearchResponse? response, CancellationToken cancellationToken)
        {
            IEnumerable<Invoice> invoices = await GetInvoicesAsync(response.Items, cancellationToken).ConfigureAwait(false);
            activity?.AddTag("rsi.response", "found");
            return (invoices.ToList(), false);
        }

        private (List<Invoice> invoices, bool isError) ProcessErrorResponse(System.Diagnostics.Activity? activity, RawTicketSearchResponse? response)
        {
            // expected: "Traffic violation ticket not found. Traffic violation ticket numbers that begin with 'E' or 'S' are available to pay online. Please allow 48 hours for your ticket to appear. If you wish to pay now and your ticket is not available, please use one of the other methods of payments listed on the ticket"
            if (response.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogDebug("Ticket not found, Road Safety service returned error message {Error}", response.Error);
                activity?.AddTag("rsi.response", "not found");
                return ([], false); // this is not an error, normal for tickets searched for to be not found
            }

            // not the error message we were expecting, log out at information level
            _logger.LogInformation("Road Safety service returned error message {Error}", response.Error);
            activity?.AddTag("rsi.response", "error");
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
            return ([], true);
        }

        private async Task<IEnumerable<Invoice>> GetInvoicesAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
        {
            System.Diagnostics.Activity? activity = System.Diagnostics.Activity.Current;

            List<string> invoiceNumbers = items
                .Select(item => GetInvoiceNumber(item?.SelectedInvoice?.Reference))
                .Where(invoiceNumber => invoiceNumber != string.Empty)
                .OrderBy(invoiceNumber => invoiceNumber)
                .ToList();

            if (invoiceNumbers.Count == 0)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                _logger.LogInformation("Could not get ticket invoice numbers, returning empty result");
                return [];
            }

            try
            {
                List<Invoice> invoices = [];

                foreach (var invoiceNumber in invoiceNumbers)
                {
                    var invoice = await _api.GetInvoice(invoiceNumber, cancellationToken).ConfigureAwait(false);
                    invoices.Add(invoice);
                }

                return invoices;
            }
            catch (Refit.ApiException exception) when (exception.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                // this is usually when the service is down for maintenance
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                _logger.LogError(exception, "Error searching for RSI ticket, service is unavailable");
                throw new TicketSearchServiceUnavailableException("Error searching for RSI ticket", exception);
            }
            catch (HttpRequestException exception)
            {
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                throw new TicketSearchErrorException("Error searching for RSI ticket", exception);
            }
        }

        private string GetInvoiceNumber(string? reference)
        {
            if (reference is null || reference.Length == 0)
            {
                _logger.LogDebug("Invoice reference is null or empty, returning empty string");
                return string.Empty;
            }

            // Reference number is ends something similar to this https://.../ride/paybc/paymentsvc/api/v1/ticket/AA000000001

            var lastSlashIndex = reference.LastIndexOf('/');
            if (lastSlashIndex != -1 && lastSlashIndex < reference.Length - 1)
            {
                string invoiceNumber = reference.Substring(lastSlashIndex + 1);
                return invoiceNumber;
            }

            return string.Empty;
        }
    }
}
