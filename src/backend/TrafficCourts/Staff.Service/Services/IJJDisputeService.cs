using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

public interface IJJDisputeService
{
    /// <summary>Returns all the existing JJ disputes from the database.</summary>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="jjGroupAssignedTo">If specified, will retrieve the records which are assigned to the specified jj group</param>
    /// <param name="jjAssignedTo">If specified, will retrieve the records which are assigned to the specified jj staff</param>
    /// <returns>A collection of JJDispute objects</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<ICollection<JJDispute>> GetAllJJDisputesAsync(string jjGroupAssignedTo, string jjAssignedTo, CancellationToken cancellationToken);

    /// <summary>Returns a specific JJ dispute from the database.</summary>
    /// <param name="id">Unique identifier of a JJ Dispute record.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>OK</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<JJDispute> GetJJDisputeAsync(string id, CancellationToken cancellationToken);
}
