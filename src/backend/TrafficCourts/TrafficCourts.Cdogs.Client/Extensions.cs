using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Configuration.Validation;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class Extensions
{
    public static IServiceCollection AddDocumentGenerationService(this IServiceCollection services, string section)
    {
        // setup the token cache
        services.AddMemoryCache();
        services.AddTransient<ITokenCache, TokenCache>();

        services.AddHttpClient("CDOGS")
            .AddStandardResilienceHandler(); 

        // setup the hosted service used to refresh the access token
        services.AddHostedService<CdogsTokenRefreshService>(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var cache = serviceProvider.GetRequiredService<ITokenCache>();
            var options = GetOidcConfiguration(serviceProvider, section);
            var logger = serviceProvider.GetRequiredService<ILogger<CdogsTokenRefreshService>>();

            return new CdogsTokenRefreshService(factory, "CDOGS", TimeProvider.System, cache, options, logger);
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
        // we are not using ConfigureValidatableSetting because there may be multiple instances of OIDC clients
        var section = configuration.GetSection(sectionName);
        var oidc = new OidcConfidentialClientConfiguration();
        section.Bind(oidc);

        // validate
        if (string.IsNullOrEmpty(oidc.ClientId)) throw new SettingsValidationException(sectionName, nameof(OidcConfidentialClientConfiguration.ClientId), "is required");
        if (string.IsNullOrEmpty(oidc.ClientSecret)) throw new SettingsValidationException(sectionName, nameof(OidcConfidentialClientConfiguration.ClientSecret), "is required");
        if (oidc.TokenEndpoint is null) throw new SettingsValidationException(sectionName, nameof(OidcConfidentialClientConfiguration.TokenEndpoint), "is required");

        return oidc;
    }

    private static OidcConfidentialClientDelegatingHandler CreateOidcDelegatingHandler(IServiceProvider serviceProvider, string sectionName)
    {
        var configuration = GetOidcConfiguration(serviceProvider, sectionName);
        var tokenCache = serviceProvider.GetRequiredService<ITokenCache>();
        var logger = serviceProvider.GetRequiredService<ILogger<OidcConfidentialClientDelegatingHandler>>();

        var handler = new OidcConfidentialClientDelegatingHandler(configuration, tokenCache, logger);
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


public class CdogsTokenRefreshService : TokenRefreshService<CdogsTokenRefreshService>
{
    public CdogsTokenRefreshService(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        TimeProvider timeProvider, 
        ITokenCache cache, 
        OidcConfidentialClientConfiguration configuration, 
        ILogger<CdogsTokenRefreshService> logger) : base(httpClientFactory, httpClientName, timeProvider, cache, configuration, logger)
    {
    }
}
