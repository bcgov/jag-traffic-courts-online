using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class Extensions
{
    public static IServiceCollection AddDocumentGenerationService(this IServiceCollection services, string section, IConfiguration configuration)
    {
        // setup the token cache
        services.AddMemoryCache();
        services.AddTransient<ITokenCache, TokenCache>();

        // setup the hosted service used to refresh the access token
        services.AddHostedService<CdogsTokenRefreshService>(serviceProvider =>
        {
            var cache = serviceProvider.GetRequiredService<IMemoryCache>();
            var options = GetOidcConfiguration(serviceProvider, section);
            var logger = serviceProvider.GetRequiredService<ILogger<CdogsTokenRefreshService>>();

            return new CdogsTokenRefreshService(cache, options, logger);
        });

        // register HttpClient for client ObjectManagementClient
        services
            .AddHttpClient<IDocumentGenerationClient, DocumentGenerationClient>()
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var configuration = GetDocumentGenerationClientConfiguration(serviceProvider, section);

                httpClient.BaseAddress = configuration.Endpoint;
            })
            .AddHttpMessageHandler((serviceProvider) => CreateOidcDelegatingHandler(serviceProvider, section));

        services.AddTransient<IDocumentGenerationService, DocumentGenerationService>();

        return services;
    }

    private static OidcConfidentialClientConfiguration GetOidcConfiguration(IServiceProvider serviceProvider, string sectionName)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var oidc = GetOidcConfiguration(configuration, sectionName);
        return oidc;
    }

    private static OidcConfidentialClientConfiguration GetOidcConfiguration(IConfiguration configuration, string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        var oidc = new OidcConfidentialClientConfiguration();
        section.Bind(oidc);

        return oidc;
    }

    private static OidcConfidentialClientDelegatingHandler CreateOidcDelegatingHandler(IServiceProvider services, string sectionName)
    {
        var configuration = GetOidcConfiguration(services, sectionName);
        var tokenCache = services.GetRequiredService<ITokenCache>();

        var handler = new OidcConfidentialClientDelegatingHandler(configuration, tokenCache);
        return handler;
    }


    private static DocumentGenerationClientConfiguration GetDocumentGenerationClientConfiguration(IServiceProvider serviceProvider, string section)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        DocumentGenerationClientConfiguration options = new();
        configuration.GetSection(section).Bind(options);

        return options;
    }
}


public class CdogsTokenRefreshService : TokenRefreshService
{
    public CdogsTokenRefreshService(IMemoryCache memoryCache, OidcConfidentialClientConfiguration configuration, ILogger<TokenRefreshService> logger) 
        : base(memoryCache, configuration, logger)
    {
    }
}
