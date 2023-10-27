using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Core.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class Extensions
{
    public static IServiceCollection AddDocumentGenerationService(this IServiceCollection services, string section)
    {
        services.AddMemoryCache();

        services.AddTransient(serviceProvider =>
        {
            IOidcConfidentialClient oidc = CreateOidcConfidentialClient(serviceProvider, section);
            return new OidcConfidentialClientDelegatingHandler(oidc);
        });

        // register HttpClient for client ObjectManagementClient
        services
            .AddHttpClient<IDocumentGenerationClient, DocumentGenerationClient>()
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var configuration = GetDocumentGenerationClientConfiguration(serviceProvider, section);

                httpClient.BaseAddress = configuration.Endpoint;
            })
            .AddHttpMessageHandler<OidcConfidentialClientDelegatingHandler>(); // depends on IOidcConfidentialClient

        services.AddTransient<IDocumentGenerationService, DocumentGenerationService>();

        return services;
    }

    private static IOidcConfidentialClient CreateOidcConfidentialClient(IServiceProvider serviceProvider, string section)
    {
        // get the named configuration
        var configuration = GetOidcConfidentialClientConfiguration(serviceProvider, section);

        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

        var logger = serviceProvider.GetRequiredService<ILogger<OidcConfidentialClient>>();

        var oidc = new OidcConfidentialClient(configuration, memoryCache, logger);
        return oidc;
    }

    private static OidcConfidentialClientConfiguration GetOidcConfidentialClientConfiguration(IServiceProvider serviceProvider, string section)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        OidcConfidentialClientConfiguration options = new();
        configuration.GetSection(section).Bind(options);

        return options;
    }

    private static DocumentGenerationClientConfiguration GetDocumentGenerationClientConfiguration(IServiceProvider serviceProvider, string section)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        DocumentGenerationClientConfiguration options = new();
        configuration.GetSection(section).Bind(options);

        return options;
    }
}
