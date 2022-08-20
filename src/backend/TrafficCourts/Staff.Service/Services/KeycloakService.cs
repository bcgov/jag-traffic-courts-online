using MassTransit;
using TrafficCourts.Common.Authentication;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;

namespace TrafficCourts.Staff.Service.Services;

public class KeycloakService : IKeycloakService
{
    private readonly IKeycloakAdminApiClient _keycloakAdminClient;
    private readonly KeycloakOptions _options;
    private readonly ILogger<KeycloakService> _logger;

    public KeycloakService(IKeycloakAdminApiClient keycloakAdminApiClient, KeycloakOptions options, ILogger<KeycloakService> logger)
    {
        _keycloakAdminClient = keycloakAdminApiClient ?? throw new ArgumentNullException(nameof(keycloakAdminApiClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<UserRepresentation>> UsersByGroupAsync(string groupName, CancellationToken cancellationToken)
    {
        string? realm = _options.Realm;

        // Find all groups that where the groupName has some sort of match in keycloak.  The Keycloak API will return both admin-vtc-staff and vtc-staff when querying for vtc-staff.
        ICollection<GroupRepresentation> groupRepresentations = await _keycloakAdminClient.GroupsAll2Async(groupName, null, null, null, realm, cancellationToken);

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

        return new List<UserRepresentation>();
    }
}
