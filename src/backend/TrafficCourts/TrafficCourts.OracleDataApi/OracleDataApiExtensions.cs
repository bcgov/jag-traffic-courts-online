using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrafficCourts.Interfaces;
using TrafficCourts.OracleDataApi;
using TrafficCourts.OracleDataApi.Client.V1;

namespace Microsoft.Extensions.DependencyInjection;

public static class OracleDataApiExtensions
{
    /// <summary>
    /// Adds the services required for accessing the Oracle Data API.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddOracleDataApi(this IServiceCollection services, IConfiguration configuration, Action<IHttpClientBuilder>? configureHttpClient = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        AddOracleDataApiClient(services, configuration, configureHttpClient); // add stuff to resolve IOracleDataApiClient
        AddOracleDataApiService(services); // add stuff to resolve IOracleDataApiClient

        AddAutoMapper(services);
        AddMediator(services);

        return services;
    }

    private static void AddOracleDataApiClient(IServiceCollection services, IConfiguration configuration, Action<IHttpClientBuilder>? configureHttpClient = null)
    {
        services.ConfigureValidatableSetting<OracleDataApiConfiguration>(configuration.GetRequiredSection(OracleDataApiConfiguration.Section));

        IHttpClientBuilder builder = services.AddHttpClient<IOracleDataApiClient, OracleDataApiClient>((services, client) =>
        {
            var configuration = services.GetRequiredService<OracleDataApiConfiguration>();
            client.BaseAddress = new Uri(configuration.BaseUrl);
        });

        // add the caller's customizations
        configureHttpClient?.Invoke(builder);

        // register instance for tracking metrics to the Oracle Data API
        services.AddSingleton<IOracleDataApiOperationMetrics, OracleDataApiOperationMetrics>();

        // decorate will replace injected instances of IOracleDataApiClient with the TimedOracleDataApiClient
        // except for the TimedOracleDataApiClient type
        services.Decorate<IOracleDataApiClient, TimedOracleDataApiClient>();
    }

    private static void AddOracleDataApiService(IServiceCollection services)
    {
        services.AddTransient<EnumTypeConverter>();
        services.AddTransient<IOracleDataApiService, OracleDataApiService>();
    }

    private static void AddAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<OracleDomainModelMappingProfile>();
        });
    }

    private static void AddMediator(IServiceCollection services)
    {
        MediatRServiceConfiguration serviceConfiguration = new();

        // register Mediatr because we depend on IMediator. There is no easy way to add just this 
        // https://github.com/jbogard/MediatR/blob/88c4d73ce514a64a9083aeb8df2e2c3692049b7d/src/MediatR/Registration/ServiceRegistrar.cs#L225

        services.TryAdd(new ServiceDescriptor(typeof(IMediator), serviceConfiguration.MediatorImplementationType, serviceConfiguration.Lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), serviceConfiguration.Lifetime));
    }

    /// <summary>
    /// Adds the AutoMapper profiles used in the Oracle Data API.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static MapperConfigurationExpression AddOracleDataApiProfiles(this MapperConfigurationExpression configuration)
    {
        configuration.AddProfile<OracleDomainModelMappingProfile>();
        return configuration;
    }

    /// <summary>
    /// Gets the list of meters exposed in this assembly
    /// </summary>
    public static IEnumerable<string> MetricMeters
    {
        get
        {
            yield return OracleDataApiOperationMetrics.MeterName;
        }
    }
}
