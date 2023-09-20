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
    public static IServiceCollection AddDocumentGenerationService(this IServiceCollection services,  IConfiguration configuration, string section)
    {
        services.AddMemoryCache();

        // bind the configuration
        services.Configure<OidcConfidentialClientConfiguration>(section, configuration.GetSection(section));
        services.Configure<DocumentGenerationClientConfiguration>(section, configuration.GetSection(section));

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
                var options = serviceProvider.GetRequiredService<IOptionsMonitor<DocumentGenerationClientConfiguration>>();
                var configuration = options.Get(section);

                httpClient.BaseAddress = configuration.Endpoint;
            })
            .AddHttpMessageHandler<OidcConfidentialClientDelegatingHandler>(); // depends on IOidcConfidentialClient

        services.AddTransient<IDocumentGenerationService, DocumentGenerationService>();

        return services;
    }

    private static IOidcConfidentialClient CreateOidcConfidentialClient(IServiceProvider serviceProvider, string section)
    {
        // get the named configuration
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OidcConfidentialClientConfiguration>>();
        var configuration = options.Get(section);

        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

        var logger = serviceProvider.GetRequiredService<ILogger<OidcConfidentialClient>>();

        var oidc = new OidcConfidentialClient(configuration, memoryCache, logger);
        return oidc;
    }
}
