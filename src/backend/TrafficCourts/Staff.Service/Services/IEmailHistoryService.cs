using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

public interface IEmailHistoryService
{
    /// <summary>Returns all the existing file history records from the database for a specified ticketNumber.</summary>
    /// <param name="ticketNumber"></param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A collection of EmailHistory objects</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<ICollection<EmailHistory>> GetEmailHistoryForTicketAsync(String ticketNumber, CancellationToken cancellationToken);
}
