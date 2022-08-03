using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace TrafficCourts.Common.Authorization;

/// <summary>
/// Provides Keycloak <see cref="AuthorizationPolicy"/>.
/// </summary>
public class KeycloakAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IOptions<KeycloakAuthorizationOptions> _options;
    private readonly IOptions<AuthorizationOptions> _authorizationOptions;
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="authorizationOptions"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public KeycloakAuthorizationPolicyProvider(IOptions<KeycloakAuthorizationOptions> options, IOptions<AuthorizationOptions> authorizationOptions)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _authorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(_authorizationOptions);
    }

    /// <summary>
    /// Gets the default authorization policy.
    /// </summary>
    /// <returns>The default authorization policy.</returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    /// <summary>
    /// Gets the fallback authorization policy.
    /// </summary>
    /// <returns>The fallback authorization policy.</returns>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

    /// <summary>
    /// Gets a AuthorizationPolicy for the given policyName
    /// </summary>
    /// <param name="policyName"></param>
    /// <returns>Gets a AuthorizationPolicy for the given policyName</returns>
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (_authorizationOptions.Value.GetPolicy(policyName) is not null)
        {
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        var builder = new AuthorizationPolicyBuilder();
        builder.AuthenticationSchemes.Add(_options.Value.RequiredScheme);
        builder.AddRequirements(new KeycloakAuthorizationRequirement(policyName));

        AuthorizationPolicy? policy = builder.Build();

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return Task.FromResult(policy);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }
}