using Microsoft.AspNetCore.Authorization;

namespace TrafficCourts.Common.Authorization;

public class KeycloakAuthorizationRequirement : IAuthorizationRequirement
{
    /// Initializes a new instance of the <see cref="KeycloakRequirement"/> class.
    /// </summary>
    /// <param name="policyName">Name of the policy.</param>
    public KeycloakAuthorizationRequirement(string policyName)
    {
        if (policyName is null)
        {
            throw new ArgumentNullException(nameof(policyName));
        }

        if (string.IsNullOrWhiteSpace(policyName))
        {
            throw new ArgumentException("Policy name cannot be empty or whitespace", nameof(policyName));
        }

        PolicyName = policyName;
    }

    /// <summary>
    /// Gets the name of the policy.
    /// </summary>
    public string PolicyName { get; }
}
