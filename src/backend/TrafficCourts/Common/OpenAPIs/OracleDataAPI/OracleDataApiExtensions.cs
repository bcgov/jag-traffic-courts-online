using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Common.OpenAPIs.OracleDataAPI;

namespace Microsoft.Extensions.DependencyInjection;

public static class OracleDataApiExtensions
{
    /// <summary>
    /// Adds the Oracle Data API client to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static IHttpClientBuilder AddOracleDataApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }


        services.ConfigureValidatableSetting<OracleDataApiConfiguration>(configuration.GetRequiredSection(OracleDataApiConfiguration.Section));
        IHttpClientBuilder builder = services.AddHttpClient<IOracleDataApiClient, OracleDataApiClient>((services, client) =>
        {
            var configuration = services.GetRequiredService<OracleDataApiConfiguration>();
            client.BaseAddress = new Uri(configuration.BaseUrl);
        });

        return builder;
    }
}
