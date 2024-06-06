using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        ArgumentNullException.ThrowIfNull(nameof(services));
        ArgumentNullException.ThrowIfNull(nameof(configure));

        services.Configure(configure);
        services.AddHttpContextAccessor();

        services.AddHttpClient<IAuthorizationHandler, KeycloakAuthorizationHandler>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<KeycloakAuthorizationOptions>>();
            client.BaseAddress = new Uri(options.Value.TokenEndpoint);
        });

        services.AddHttpClient<IKeycloakAuthorizationService, KeycloakAuthorizationService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<KeycloakAuthorizationOptions>>();
            client.BaseAddress = new Uri(options.Value.TokenEndpoint);
        }).AddAuthorizationHeaderPropagation();

        return services;
    }
}
