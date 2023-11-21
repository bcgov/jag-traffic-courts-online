using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace TrafficCourts.Common.Authentication;

public static class KeycloakExtension
{
    public static IServiceCollection AddKeycloakAdminApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        string section = "OAuth"; // TODO: switch to Keycloak or something

        services.ConfigureValidatableSetting<KeycloakOptions>(configuration.GetRequiredSection(KeycloakOptions.Section));

        services.AddMemoryCache();
        services.AddTransient<ITokenCache, TokenCache>();
        services.AddHostedService(serviceProvider => CreateTokenRefreshService(serviceProvider, section));
        services.AddHttpClient<IKeycloakAdminApiClient, KeycloakAdminApiClient>(ConfigureAdminClient)
            .AddHttpMessageHandler((serviceProvider) => CreateOidcDelegatingHandler(serviceProvider, section));

        return services;
    }

    private static void ConfigureAdminClient(IServiceProvider serviceProvider, HttpClient client)
    {
        KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
        client.BaseAddress = new Uri($"{keycloakOptions.BaseUri}");
    }

    private static OidcConfidentialClientDelegatingHandler CreateOidcDelegatingHandler(IServiceProvider serviceProvider, string sectionName)
    {
        var configuration = GetConfiguration(serviceProvider, sectionName);
        var tokenCache = serviceProvider.GetRequiredService<ITokenCache>();

        var handler = new OidcConfidentialClientDelegatingHandler(configuration, tokenCache);
        return handler;
    }

    private static KeycloakTokenRefreshService CreateTokenRefreshService(IServiceProvider serviceProvider, string sectionName)
    {
        var cache = serviceProvider.GetRequiredService<IMemoryCache>();
        var configuration = GetConfiguration(serviceProvider, sectionName);
        var logger = serviceProvider.GetRequiredService<ILogger<KeycloakTokenRefreshService>>();

        return new KeycloakTokenRefreshService(cache, configuration, logger);
    }

    private static OidcConfidentialClientConfiguration GetConfiguration(IServiceProvider serviceProvider, string sectionName)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection(sectionName);
        var oidc = new OidcConfidentialClientConfiguration();
        section.Bind(oidc);

        return oidc;
    }
}
