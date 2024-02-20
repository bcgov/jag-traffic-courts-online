using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;

namespace TrafficCourts.Common.OpenAPIs.Keycloak.v18_0;

public static class KeycloakExtensions
{
    public static ICollection<string> GetAttributeValues(this UserRepresentation userRepresentation, string key)
    {
        if (userRepresentation is null)
        {
            throw new ArgumentNullException(nameof(userRepresentation));
        }

        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
        }

        if (userRepresentation.Attributes.TryGetValue(key, out ICollection<string>? partIds))
        {
            return partIds;
        }
        else
        {
            return [];
        }
    }
}
