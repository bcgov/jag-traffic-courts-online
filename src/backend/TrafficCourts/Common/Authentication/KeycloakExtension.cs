using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;

namespace TrafficCourts.Common.Authentication;

public static class KeycloakExtension
{
    public static IServiceCollection AddKeycloakAdminApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureValidatableSetting<OAuthOptions>(configuration.GetRequiredSection(OAuthOptions.Section));
        services.ConfigureValidatableSetting<KeycloakOptions>(configuration.GetRequiredSection(KeycloakOptions.Section));

        services.AddMemoryCache();
        services.AddSingleton<ITokenCache<OAuthOptions, Token>, OAuthTokenCache>();
        services.AddHttpClient<IOAuthClient, OAuthClient>(ConfigureOAuthClient);       
        services.AddTransient<ITokenService, OAuthTokenService>();
        services.AddTransient<TokenAuthorizationHandler>();
        services.AddHttpClient<IKeycloakAdminApiClient, KeycloakAdminApiClient>(ConfigureAdminClient)
            .AddHttpMessageHandler<TokenAuthorizationHandler>();

        return services;
    }

    private static void ConfigureOAuthClient(IServiceProvider services, HttpClient client)
    {
        OAuthOptions oAuthOptions = services.GetRequiredService<IOptions<OAuthOptions>>().Value;
        client.BaseAddress = new Uri($"{oAuthOptions.TokenUri}");
    }

    private static void ConfigureAdminClient(IServiceProvider services, HttpClient client)
    {
        KeycloakOptions keycloakOptions = services.GetRequiredService<IOptions<KeycloakOptions>>().Value;
        client.BaseAddress = new Uri($"{keycloakOptions.BaseUri}");
    }
}
