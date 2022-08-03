using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using TrafficCourts.Common.Authorization;

namespace TrafficCourts.Staff.Service.Authentication;

public static class AuthenticationExtensions
{
    private const string _jwtBearerOptionsSection = "Jwt";
    private const string _roleClaimType = "role";

    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Note: AddJwtBearer does not use IConfigureOptions<JwtBearerOptions>
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                configuration.Bind(_jwtBearerOptionsSection, options);

                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = _roleClaimType;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context => await OnTokenValidatedAsync(context)
                };
            });
    }

    /// <summary>
    /// Adds authorization policy services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns></returns>
    public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(_jwtBearerOptionsSection).Get<JwtBearerOptions>();

        services.AddAuthorization(options =>
        {
            //options.AddPolicy(Policies.CanAssignDisputes, policy => policy.RequiresKeycloakEntitlement("Disputes", "Assign"));

        }).AddKeycloakAuthorization(options =>
        {
            options.Audience = jwtOptions.Audience;
            options.TokenEndpoint = $"{jwtOptions.Authority}/protocol/openid-connect/token";
        });

        return services;
    }


    private static Task OnTokenValidatedAsync(TokenValidatedContext context)
    {
        if (context.SecurityToken is JwtSecurityToken accessToken
                && context?.Principal?.Identity is ClaimsIdentity identity
                && identity.IsAuthenticated)
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, accessToken.Subject));
            
            FlattenResourceAccessRoles(identity, context?.Options?.Audience);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Flattens the Realm Access claim, as Microsoft Identity Model doesn't support nested claims
    /// </summary>
    private static void FlattenResourceAccessRoles(ClaimsIdentity identity, string? audience)
    {
        if (string.IsNullOrWhiteSpace(audience))
        {
            return;
        }

        // JSON is e.g. "resource_access" : { "tco-staff-portal" : { "roles" : [ "vtc-user" ] }, "account" : { "roles" : [ "guest", "admin" ] } }
        // look for audience tco-staff-portal under resource_access (line 62), then add those roles which are listed uder tco-staff-portal as claims to identity (line 66)
        var resourceAccessClaim = identity.Claims
            .SingleOrDefault(claim => claim.Type == Claims.ResourceAccess)
            ?.Value;

        if (resourceAccessClaim != null)
        {
            var audiencesRoles = JsonSerializer.Deserialize<Dictionary<string, ResourceAccess>>(resourceAccessClaim);
            if (audiencesRoles != null) foreach(var audienceRole in audiencesRoles)
            {
                if (audienceRole.Key == audience && audienceRole.Value.roles != null)
                {
                    foreach(var role in audienceRole.Value.roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }
            }
        }
    }

    internal class ResourceAccess
    {
        public string[]? roles { get; set; }
    }
}
