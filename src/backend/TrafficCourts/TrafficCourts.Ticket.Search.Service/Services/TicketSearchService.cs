using Grpc.Core;

namespace TrafficCourts.Ticket.Search.Service.Services
{
    public class TicketSearchService : TicketSearch.TicketSearchBase
    {
        private readonly ILogger<TicketSearchService> _logger;
        public TicketSearchService(ILogger<TicketSearchService> logger)
        {
            _logger = logger;
        }

        public override Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
        {
            return Task.FromResult(new SearchReply
            {
                Message = "Hello " + request.Number
            });
        }
    }
}