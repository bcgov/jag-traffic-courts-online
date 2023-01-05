using Microsoft.Extensions.Configuration;
using TrafficCourts.Coms.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class Extensions
{
    /// <summary>
    /// Adds the Object Management Service to the services collection. <see cref="IObjectManagementService"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="section">The section containing <see cref="ObjectManagementServiceConfiguration"/></param>
    /// <returns></returns>
    public static IServiceCollection AddObjectManagementService(this IServiceCollection services, string section)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddHttpClient<ObjectManagementClient>((provider, client) =>
        {
            ObjectManagementServiceConfiguration options = GetConfiguration(provider, section);

            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(options.Username, options.Password);
        });

        services.AddTransient<IObjectManagementClient>(provider =>
        {
            ObjectManagementServiceConfiguration options = GetConfiguration(provider, section);

            var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient(nameof(ObjectManagementClient));
            ObjectManagementClient client = new(httpClient);

            // BaseUrl defaults to "/api/v1", so append to our url
            var baseUrl = new Uri(options.BaseUrl).ToString().TrimEnd('/');

            client.BaseUrl = baseUrl + client.BaseUrl;
            return client;
        });

        services.AddTransient<IObjectManagementService, ObjectManagementService>();

        return services;
    }

    private static ObjectManagementServiceConfiguration GetConfiguration(IServiceProvider provider, string section)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        ObjectManagementServiceConfiguration options = new();
        configuration.GetSection(section).Bind(options);

        return options;
    }
}
