namespace TrafficCourts.Ticket.Search.Service.Features.Search.Mock
{
    public class MockTicketSearchService : ITicketSearchService
    {
        public Task<IEnumerable<Invoice>> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
