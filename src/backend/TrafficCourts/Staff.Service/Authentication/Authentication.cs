using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using TrafficCourts.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Routing;

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

        services.AddTransient<IClaimsTransformation>(_ => new KeycloakRolesClaimsTransformation(_roleClaimType, jwtOptions.Audience!));

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

        if (!string.IsNullOrEmpty(jwtOptions.Authority) && jwtOptions.Authority.EndsWith('/'))
        {
            jwtOptions.Authority = jwtOptions.Authority[..^1];
        }

        services
            .AddAuthorization(options => ApplyPolicies(options))
            .AddKeycloakAuthorization(options =>
        {
            options.Audience = jwtOptions.Audience!; // Audience is required configuration
            options.TokenEndpoint = $"{jwtOptions.Authority}/protocol/openid-connect/token";
        });

        return services;
    }

    /// <summary>
    /// Gets the policies defined on controllers.
    /// </summary>
    /// <param name="options"></param>
    private static void ApplyPolicies(AuthorizationOptions options)
    {
        var policies = GetPolicies();

        foreach (var authorizationPolicy in policies)
        {
            options.AddPolicy(authorizationPolicy.Policy!, policy => policy.RequiresKeycloakEntitlement(authorizationPolicy.Resource, authorizationPolicy.Scope));
        }
    }

    private static IEnumerable<KeycloakAuthorizeAttribute> GetPolicies()
    {
        // find all controllers and public controller methods looking for KeycloakAuthorizeAttribute and apply
        var controllerBaseType = typeof(ControllerBase);

        // assume all controllers are defined in this assembly
        IEnumerable<Type> controllers = typeof(Program)
            .Assembly
            .GetTypes()
            .Where(type => controllerBaseType.IsAssignableFrom(type) && !type.IsAbstract);

        // HashSet will remove any duplicates
        HashSet<KeycloakAuthorizeAttribute> policies = new HashSet<KeycloakAuthorizeAttribute>();

        foreach (var controller in controllers)
        {
            var attributes = controller.GetCustomAttributes<KeycloakAuthorizeAttribute>();
            foreach (var attribute in attributes)
            {
                policies.Add(attribute);
            }

            var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                // check if the method is a action method
                if (method.GetCustomAttribute<HttpMethodAttribute>() is not null)
                {
                    attributes = method.GetCustomAttributes<KeycloakAuthorizeAttribute>();
                    foreach (var attribute in attributes)
                    {
                        policies.Add(attribute);
                    }
                }
            }
        }

        return policies;
    }
}
