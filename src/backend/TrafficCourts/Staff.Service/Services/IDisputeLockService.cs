using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;
using TrafficCourts.Staff.Service.Models;

namespace TrafficCourts.Staff.Service.Services;

public interface IDisputeLockService
{
    /// <summary>
    /// Inserts a lock record for a JJDispute based on the given disputeId and username.
    /// </summary>
    /// <param name="disputeId"></param>
    /// <param name="username"></param>
    /// <returns>The saved lock.</returns>
    Lock? GetLock(long disputeId, string username);

    /// <summary>
    /// Refreshes an existing lock's expiry time by extending it.
    /// </summary>
    /// <param name="lockId"></param>
    /// <param name="username"></param>
    /// <returns>Refreshed lock expiry time</returns>
    DateTimeOffset? RefreshLock(Guid lockId, string username);

    /// <summary>
    /// Removes an existing lock from the database.
    /// </summary>
    /// <param name="lockId"></param>
    void ReleaseLock(Guid lockId);

}
