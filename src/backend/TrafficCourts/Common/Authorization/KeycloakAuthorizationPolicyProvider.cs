using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace TrafficCourts.Common.Authorization;

public class KeycloakAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IOptions<KeycloakAuthorizationOptions> _options;
    private readonly IOptions<AuthorizationOptions> _authorizationOptions;
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public KeycloakAuthorizationPolicyProvider(IOptions<KeycloakAuthorizationOptions> options, IOptions<AuthorizationOptions> authorizationOptions)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _authorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(_authorizationOptions);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();

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