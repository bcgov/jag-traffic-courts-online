using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi
{
    public class RoadSafetyTicketSearchService : ITicketInvoiceSearchService
    {
        private readonly IRoadSafetyTicketSearchApi _api;
        private readonly IFusionCache _cache;
        private readonly ILogger<RoadSafetyTicketSearchService> _logger;

        public RoadSafetyTicketSearchService(
            IRoadSafetyTicketSearchApi api,
            IFusionCache cache,
            ILogger<RoadSafetyTicketSearchService> logger)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IList<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey(ticketNumber, issuedTime);

            var invoices = await _cache.GetOrSetAsync<List<Invoice>>(
                cacheKey,
                FusionCacheSearchAsync,
                token: cancellationToken);

            return invoices;

            // determine the cache duration based on the result
            // see: https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/AdaptiveCaching.md
            TimeSpan GetCacheDuration(IList<Invoice>? result)
            {
                // wasn't found, cache for 5 minutes, otherwise 1 day - this helps avoid DDoS
                return result is null || result.Count == 0
                    ? TimeSpan.FromMinutes(5)
                    : TimeSpan.FromDays(1);
            }

            async Task<List<Invoice>> FusionCacheSearchAsync(FusionCacheFactoryExecutionContext<List<Invoice>> context, CancellationToken cancellationToken)
            {
                var (invoices, isError) = await SearchRsiAsync(ticketNumber, issuedTime, cancellationToken).ConfigureAwait(false);
                
                if (isError)
                {
                    context.Options.Duration = TimeSpan.Zero; // if the service is having an issue dont cache
                }
                else
                {
                    context.Options.Duration = GetCacheDuration(invoices);
                }

                return invoices;
            }
        }

        private string CacheKey(string ticketNumber, TimeOnly issuedTime) => Caching.Cache.TicketSearch.Key(ticketNumber, issuedTime);

        private async Task<(List<Invoice> invoices, bool isError)> SearchRsiAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            using var activity = Diagnostics.Source.StartActivity("rsi ticket search");

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
                // TODO: should we be catching ApiException?
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
                return ([], true);
            }

            if (response is not null && response.Items is not null)
            {
                IEnumerable<Invoice> invoices = await GetInvoicesAsync(response.Items, cancellationToken).ConfigureAwait(false);
                return (invoices.ToList(), false);
            }

            _logger.LogInformation("No invoice numbers returned, returning empty result");
            return ([], false);

        }

        private async Task<IEnumerable<Invoice>> GetInvoicesAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
        {
            var invoiceNumbers = items.Select(item => GetInvoiceNumber(item?.SelectedInvoice?.Reference))
                .Where(invoiceNumber => invoiceNumber != string.Empty)
                .ToList();

            if (invoiceNumbers.Count == 0)
            {
                _logger.LogInformation("Could not get ticket invoice numbers, returning empty result");
                return Enumerable.Empty<Invoice>();
            }

            List<Task<Invoice>> getInvoiceTasks = new List<Task<Invoice>>(invoiceNumbers.Count);

            foreach (var invoiceNumber in invoiceNumbers)
            {
                getInvoiceTasks.Add(Task.Run(async () => await _api.GetInvoice(invoiceNumber, cancellationToken).ConfigureAwait(false)));
            }

            var groupedTasks = Task.WhenAll(getInvoiceTasks);

            try
            {
                var invoices = await groupedTasks.ConfigureAwait(false);
                return invoices.OrderBy(_ => _.InvoiceNumber);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to get invoices, returning empty result");
                return Array.Empty<Invoice>();
            }
        }

        private static string GetInvoiceNumber(string? reference)
        {
            if (reference is null || reference.Length == 0) return string.Empty;

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
