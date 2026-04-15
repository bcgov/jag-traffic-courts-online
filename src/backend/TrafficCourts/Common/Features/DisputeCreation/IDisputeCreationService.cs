namespace TrafficCourts.Common.Features.DisputeCreation
{
    public interface IDisputeCreationService
    {
        /// <summary>
        /// Determines if a dispute can be created for a ticket.
        /// </summary>
        /// <param name="ticketNumber">The number of the ticket.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the dispute can be created.</returns>
        Task<bool> CanCreateDispute(string ticketNumber, CancellationToken cancellationToken);
    }
}