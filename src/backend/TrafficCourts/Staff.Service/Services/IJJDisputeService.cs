using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

public interface IJJDisputeService
{
    /// <summary>Returns all the existing JJ disputes from the database.</summary>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="jjAssignedTo">If specified, will retrieve the records which are assigned to the specified jj staff</param>
    /// <returns>A collection of JJDispute objects</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<ICollection<JJDispute>> GetAllJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken);

    /// <summary>Returns a specific JJ dispute from the database.</summary>
    /// <param name="id">Unique identifier of a JJ Dispute record.</param>
    /// <param name="assignVTC">Boolean to indicate need to assign VTC staff.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>OK</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<JJDispute> GetJJDisputeAsync(string id, bool assignVTC, CancellationToken cancellationToken);

    /// <summary>Updates the properties of a particular JJ Dispute record based on the given values.</summary>
    /// <param name="ticketNumber">Unique identifier of a JJ Dispute record to modify.</param>
    /// <param name="checkVTC">Boolean to indicate need to check VTC assigned</param>
    /// <param name="jjDispute">A modified version of the JJ Dispute record to save.</param>    
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The submitted/updated JJ Dispute record.</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<JJDispute> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken);
}
