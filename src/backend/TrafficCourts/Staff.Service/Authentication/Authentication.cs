using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using TrafficCourts.Common.Authorization;

namespace TrafficCourts.Staff.Service.Authentication;

public static class AuthenticationExtensions
{
    private const string _jwtBearerOptionsSection = "Jwt";
    private const string _roleClaimType = "role";

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Note: AddJwtBearer does not use IConfigureOptions<JwtBearerOptions>
        var jwtOptions = configuration.GetSection(_jwtBearerOptionsSection).Get<JwtBearerOptions>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = jwtOptions.Authority;
                options.Audience = jwtOptions.Audience;
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = _roleClaimType;

             });

        services.AddTransient<IClaimsTransformation>(_ => new KeycloakRolesClaimsTransformation(_roleClaimType, jwtOptions.Audience));

        return services;
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
}
