using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;

namespace TrafficCourts.Staff.Service.Services;

public interface IKeycloakService
{

    /// <summary>Returns all users for a given group from Keycloak.</summary>
    /// <param name="groupName"></param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The collection of UserRepresentation records.</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<ICollection<UserRepresentation>> UsersByGroupAsync(string groupName, CancellationToken cancellationToken);

}
