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
    /// <exception cref="ArgumentNullException">Thrown if id is null</exception>
    Task<JJDispute> GetJJDisputeAsync(string id, bool assignVTC, CancellationToken cancellationToken);

    /// <summary>Updates the properties of a particular JJ Dispute record based on the given values.</summary>
    /// <param name="ticketNumber">Unique identifier of a JJ Dispute record to modify.</param>
    /// <param name="checkVTC">Boolean to indicate need to check VTC assigned</param>
    /// <param name="jjDispute">A modified version of the JJ Dispute record to save.</param>    
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The submitted/updated JJ Dispute record.</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    /// <exception cref="ArgumentNullException">Thrown if ticketNumber is null</exception>
    Task<JJDispute> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken);

    /// <summary>Updates each JJ Dispute based on the passed in IDs (ticket number) to assign them to a specific JJ or unassign them if JJ not specified.</summary>
    /// <param name="ticketNumbers">List of Unique identifiers for JJ Dispute records to be assigend/unassigned.</param>
    /// <param name="username">IDIR username of the JJ that JJ Dispute(s) will be assigned to, if specified. Otherwise JJ Disputes will be unassigned.</param>    
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns></returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    /// <exception cref="ArgumentNullException">Thrown if ticketNumbers is null</exception>
    Task AssignJJDisputesToJJ(List<string> ticketNumbers, string? username, CancellationToken cancellationToken);

    /// <summary>Updates the status of a particular JJDispute record to REVIEW as well as adds an optional remark that explaining why the status was set to REVIEW.</summary>
    /// <param name="ticketNumber">Unique identifier for a specific JJ Dispute record.</param>
    /// <param name="remark">The remark or note (max 256 characters) the JJDispute was set to REVIEW.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    /// <exception cref="ArgumentNullException">Thrown if ticketNumber is null</exception>
    Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, string remark, bool checkVTC, CancellationToken cancellationToken);

    /// <summary>Updates the status of a particular JJDispute record to ACCEPTED.</summary>
    /// <param name="ticketNumber">Unique identifier for a specific JJ Dispute record.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    /// <exception cref="ArgumentNullException">Thrown if ticketNumber is null</exception>
    Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTC, CancellationToken cancellationToken);
}
