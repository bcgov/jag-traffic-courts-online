using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace TrafficCourts.Common.Authorization;

public static class KeycloakAuthorizationServiceCollectionExtensions
{
    /// <summary>
    /// Add keycloak authorization components.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <returns></returns>
    public static IServiceCollection AddKeycloakAuthorization(this IServiceCollection services, Action<KeycloakAuthorizationOptions> configure)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        services.Configure(configure);
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthorizationHandler, KeycloakAuthorizationHandler>();

        return services;
    }
}
