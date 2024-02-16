using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v22_0;

namespace TrafficCourts.Common.OpenAPIs.Keycloak.v22_0;

public static class KeycloakExtensions
{
    public static IList<string> GetAttributeValues(this UserRepresentation userRepresentation, string key)
    {
        if (userRepresentation is null)
        {
            throw new ArgumentNullException(nameof(userRepresentation));
        }

        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
        }

        if (userRepresentation.Attributes.TryGetValue(key, out IList<string>? partIds))
        {
            return partIds;
        }
        else
        {
            return [];
        }
    }
}
