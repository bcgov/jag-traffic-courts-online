using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using TrafficCourts.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

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
    public static IServiceCollection AddAuthorization<TPolicy>(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(_jwtBearerOptionsSection).Get<JwtBearerOptions>();

        if (!string.IsNullOrEmpty(jwtOptions.Authority) && jwtOptions.Authority.EndsWith('/'))
        {
            jwtOptions.Authority = jwtOptions.Authority[..^1];
        }

        services
            .AddAuthorization(options => ApplyPolicies<TPolicy>(options))
            .AddKeycloakAuthorization(options =>
        {
            options.Audience = jwtOptions.Audience;
            options.TokenEndpoint = $"{jwtOptions.Authority}/protocol/openid-connect/token";
        });

        return services;
    }


    /// <summary>
    /// Gets the policies defined in the <typeparam name="TPolicy" />.
    /// </summary>
    /// <param name="options"></param>
    private static void ApplyPolicies<TPolicy>(AuthorizationOptions options)
    {
        // get all the public static string fields
        var fields = typeof(TPolicy)
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(_ => _.FieldType == typeof(string));

        foreach (var field in fields)
        {
            // get the value
            var value = field.GetValue(null) as string;
            if (string.IsNullOrWhiteSpace((value)))
            {
                continue;
            }

            // split on colon
            var colon = value.IndexOf(':');
            if (colon == -1)
            {
                continue;
            }

            // value is "resource:scope"
            string name = value;
            string resource = value[0..colon];
            string scope = value[(colon+1)..];

            // if propertly formatted
            if (!string.IsNullOrEmpty(resource) && !string.IsNullOrEmpty(scope))
            {
                options.AddPolicy(name, policy => policy.RequiresKeycloakEntitlement(resource, scope));
            }
        }
    }
}
