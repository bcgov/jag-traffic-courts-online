using Microsoft.AspNetCore.Authorization;

namespace TrafficCourts.Common.Authorization;

public class KeycloakAuthorizationRequirement : IAuthorizationRequirement
{
    /// Initializes a new instance of the <see cref="KeycloakRequirement"/> class.
    /// </summary>
    /// <param name="policyName">Name of the policy.</param>
    public KeycloakAuthorizationRequirement(string policyName)
    {
        PolicyName = policyName ?? throw new ArgumentNullException(nameof(policyName));
    }

    /// <summary>
    /// Gets the name of the policy.
    /// </summary>
    /// <value>
    /// The name of the policy.
    /// </value>
    public string PolicyName { get; }
}
