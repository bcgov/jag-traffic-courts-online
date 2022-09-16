using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services;

public interface IFileHistoryService
{
    /// <summary>Saves a file history record to the database for a specified ticketNumber.</summary>
    /// <param name="fileHistory"></param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Id of newly inserted file history record</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<long> SaveFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken);
}
