using TrafficCourts.Common.Authentication;
using TrafficCourts.Common.OpenAPIs.Keycloak;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v22_0;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Staff.Service.Services;

public class KeycloakService : IKeycloakService
{
    private readonly IKeycloakAdminApiClient _keycloakAdminClient;
    private readonly KeycloakOptions _options;
    private readonly ILogger<KeycloakService> _logger;
    private readonly IFusionCache _cache;

    public KeycloakService(
        IKeycloakAdminApiClient keycloakAdminApiClient, 
        KeycloakOptions options,
        IFusionCacheProvider cacheProvider,
        ILogger<KeycloakService> logger)
    {
        ArgumentNullException.ThrowIfNull(cacheProvider);

        _keycloakAdminClient = keycloakAdminApiClient ?? throw new ArgumentNullException(nameof(keycloakAdminApiClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _cache = cacheProvider.GetCache(Caching.Cache.Keycloak.Name);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<UserRepresentation>> UsersByGroupAsync(string groupName, CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Keycloak.UsersByGroup(groupName);

        var users = await _cache.GetOrSetAsync<ICollection<UserRepresentation>>(
            key,
            ct => GetKeycloakUsersByGroupAsync(groupName, ct),
            options => options.SetDuration(TimeSpan.FromMinutes(10)),
            token: cancellationToken);

        return users;
    }

    public async Task<ICollection<UserRepresentation>> UsersByIdirAsync(string idirUsername, CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Keycloak.UsersByUsername(idirUsername);

        var users = await _cache.GetOrSetAsync<ICollection<UserRepresentation>>(
            key,
            ct => GetKeycloakUsersByIdirAsync(idirUsername, ct),
            options => options.SetDuration(TimeSpan.FromMinutes(10)),
            token: cancellationToken);

        return users;
    }

    private async Task<ICollection<UserRepresentation>> GetKeycloakUsersByGroupAsync(string groupName, CancellationToken cancellationToken)
    {
        // TODO: add caching of the group lookup, the members could also be cached for a short period of time
        string? realm = _options.Realm;

        // Find all groups that where the groupName has some sort of match in keycloak.  The Keycloak API will return both admin-vtc-staff and vtc-staff when querying for vtc-staff.
        ICollection<GroupRepresentation> groupRepresentations = await _keycloakAdminClient.GroupsAll2Async(groupName, null, null, null, null, null, null, realm, cancellationToken);

        // Filter returned list to the exact name we are looking for
        GroupRepresentation? groupRepresentation = groupRepresentations
            .Where(item => groupName.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

        if (groupRepresentation is not null)
        {
            // Find all users by groupId (the uuid of the groupRepresentation record)
            ICollection<UserRepresentation> userRepresentations = await _keycloakAdminClient.MembersAsync(null, null, false, realm, groupRepresentation.Id, cancellationToken);

            return userRepresentations;
        }

        return [];
    }

    private async Task<ICollection<UserRepresentation>> GetKeycloakUsersByIdirAsync(string idirUsername, CancellationToken cancellationToken)
    {
        string? realm = _options.Realm;
        string q = $"{UserAttributes.IdirUsername}:{idirUsername}";

        var users = await _keycloakAdminClient.UsersAll3Async(null, null, null, null, null, null, null, null, null, null, null, null, null, q, realm, cancellationToken);
        return users;
    }
}
