using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Common.OpenAPIs.OracleDataAPI;
using TrafficCourts.Common.OpenAPIs.OracleDataAPI.v1_0;

namespace Microsoft.Extensions.DependencyInjection;

public static class OracleDataApiExtensions
{
    /// <summary>
    /// Adds the Oracle Data API client to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [Obsolete("Use TrafficCourts.OracleDataApi.AddOracleDataApi")]
    public static IHttpClientBuilder AddOracleDataApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.ConfigureValidatableSetting<OracleDataApiConfiguration>(configuration.GetRequiredSection(OracleDataApiConfiguration.Section));
        IHttpClientBuilder builder = services.AddHttpClient<IOracleDataApiClient, OracleDataApiClient>((services, client) =>
        {
            var configuration = services.GetRequiredService<OracleDataApiConfiguration>();
            client.BaseAddress = new Uri(configuration.BaseUrl);
        });

        // register instance for tracking metrics to the Oracle Data API
        services.AddSingleton<IOracleDataApiOperationMetrics, OracleDataApiOperationMetrics>();

        // decorate will replace injected instances of IOracleDataApiClient with the TimedOracleDataApiClient
        // except for the TimedOracleDataApiClient type
        services.Decorate<IOracleDataApiClient, TimedOracleDataApiClient>();

        return builder;
    }

    public static string GetArcDisputeType(this DisputeCount count)
    {
        if (count is not null)
        {
            if (count.RequestReduction == DisputeCountRequestReduction.Y) return "F"; // fine
            if (count.RequestTimeToPay == DisputeCountRequestTimeToPay.Y) return "F"; // fine
        }

        return "A"; // allegation
    }
}
