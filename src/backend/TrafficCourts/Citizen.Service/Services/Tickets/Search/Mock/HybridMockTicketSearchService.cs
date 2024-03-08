using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock
{
    public class HybridMockTicketSearchService : ITicketInvoiceSearchService
    {
        private readonly RoadSafetyTicketSearchService _roadSafetyTicketSearch;
        private readonly MockTicketSearchService _mockTicketSearch;
        private readonly ILogger<HybridMockTicketSearchService> _logger;

        public HybridMockTicketSearchService(
            RoadSafetyTicketSearchService roadSafetyTicketSearch, 
            MockTicketSearchService mockTicketSearch, 
            ILogger<HybridMockTicketSearchService> logger)
        {
            _roadSafetyTicketSearch = roadSafetyTicketSearch;
            _mockTicketSearch = mockTicketSearch;
            _logger = logger;
        }

        public async Task<IList<Invoice>> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
        {
            IList<Invoice> invoices;

            try
            {
                invoices = await _roadSafetyTicketSearch.SearchAsync(ticketNumber, issuedTime, cancellationToken);
                if (invoices.Count != 0)
                {
                    return invoices;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to query RSI service");
            }

            invoices = await _mockTicketSearch.SearchAsync(ticketNumber, issuedTime, cancellationToken);
            return invoices;
        }
    }
}
