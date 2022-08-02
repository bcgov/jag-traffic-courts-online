using Microsoft.AspNetCore.Authorization;

namespace TrafficCourts.Common.Authorization;

public static class KeycloakAuthorizationPolicyBuilderExtensions
{
    /// <summary>
    /// Adds a Keycloak requirement to the Requirements.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="scope">The scope.</param>
    /// <returns></returns>
    public static AuthorizationPolicyBuilder RequiresKeycloakEntitlement(this AuthorizationPolicyBuilder builder, string resource, string scope)
    {
        if (resource is null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        if (scope is null)
        {
            throw new ArgumentNullException(nameof(scope));
        }

        builder.AddRequirements(new KeycloakAuthorizationRequirement($"{resource}#{scope}"));
        return builder;
    }
}
