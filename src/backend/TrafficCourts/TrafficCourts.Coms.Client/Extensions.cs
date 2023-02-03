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
    /// <param name="memoryStreamFactory">
    /// Optional function used to create new memory streams, 
    /// allows using Microsoft.IO.RecyclableMemoryStreamManager to create instances of returned streams.
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddObjectManagementService(this IServiceCollection services, string section, Func<MemoryStream>? memoryStreamFactory = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(section);

        if (memoryStreamFactory is null)
        {
            memoryStreamFactory = () => new MemoryStream();
        }

        // register IMemoryStreamFactory
        services.AddSingleton<IMemoryStreamFactory, MemoryStreamFactory>(serviceProvider => new MemoryStreamFactory(memoryStreamFactory));

        // register HttpClient for client ObjectManagementClient
        services.AddHttpClient<ObjectManagementClient>((provider, client) =>
        {
            ObjectManagementServiceConfiguration options = GetConfiguration(provider, section);

            // TODO: check that Username and Password are not empty

            // username and password are required in this client
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(options.Username, options.Password);
        });

        // register how to create IObjectManagementClient
        services.AddTransient<IObjectManagementClient>(provider =>
        {
            ObjectManagementServiceConfiguration options = GetConfiguration(provider, section);

            // TODO: check that BaseUrl is not empty and is valid uri

            var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient(nameof(ObjectManagementClient));
            ObjectManagementClient client = new(httpClient);

            // BaseUrl defaults to "/api/v1", so append to our url
            var baseUrl = new Uri(options.BaseUrl).ToString().TrimEnd('/');

            client.BaseUrl = baseUrl + client.BaseUrl;
            return client;
        });

        // register the public interface
        services.AddTransient<IObjectManagementService, ObjectManagementService>();

        services.AddObjectManagementRepository(); // work around for coms not 

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
