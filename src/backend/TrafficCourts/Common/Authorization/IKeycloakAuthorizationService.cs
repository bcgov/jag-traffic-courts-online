
namespace TrafficCourts.Common.Authorization
{
    public interface IKeycloakAuthorizationService
    {
        Task<IList<Resource>> GetPermissionsAsync(CancellationToken cancellationToken);
    }
}