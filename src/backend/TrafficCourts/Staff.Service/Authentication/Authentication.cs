using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace TrafficCourts.Staff.Service.Authentication;

public static class Authentication
{
    public static void Initialize(IServiceCollection services, IConfiguration configuration)
    {
        // Note: AddJwtBearer does not use IConfigureOptions<JwtBearerOptions>
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                configuration.Bind("Jwt", options);

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context => await OnTokenValidatedAsync(context)
                };
            });
    }

    private static Task OnTokenValidatedAsync(TokenValidatedContext context)
    {
        if (context.SecurityToken is JwtSecurityToken accessToken
                && context?.Principal?.Identity is ClaimsIdentity identity
                && identity.IsAuthenticated)
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, accessToken.Subject));
            
            FlattenRealmAccessRoles(identity, context?.Options?.Audience);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Flattens the Realm Access claim, as Microsoft Identity Model doesn't support nested claims
    /// </summary>
    private static void FlattenRealmAccessRoles(ClaimsIdentity identity, string? audience)
    {
        if (string.IsNullOrWhiteSpace(audience))
        {
            return;
        }

        // TODO: extract role from resource_access.audience: { "roles": [ ] }

        var resourceAccessClaim = identity.Claims
            .SingleOrDefault(claim => claim.Type == Claims.ResourceAccess)
            ?.Value;

        if (resourceAccessClaim != null)
        {
            var realmAccess = JsonSerializer.Deserialize<ResourceAccess>(resourceAccessClaim);
            if (realmAccess?.Roles is not null)
            {
                identity.AddClaims(realmAccess.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }
        }
    }

    internal class ResourceAccess
    {
        public string[]? Roles { get; set; }
    }
}
