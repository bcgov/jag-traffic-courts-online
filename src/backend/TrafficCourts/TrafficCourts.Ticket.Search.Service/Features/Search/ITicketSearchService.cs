namespace TrafficCourts.Ticket.Search.Service.Features.Search
{
    public interface ITicketSearchService
    {
        Task<IEnumerable<Invoice>> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken);
    }
}
