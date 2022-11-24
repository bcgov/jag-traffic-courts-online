using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
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

    /// <summary>
    /// HTTP Basic Authentication authorization header
    /// </summary>
    /// <see cref="https://github.com/IdentityModel/IdentityModel/blob/main/src/Client/BasicAuthenticationHeaderValue.cs"/>
    /// <seealso cref="System.Net.Http.Headers.AuthenticationHeaderValue" />
    private class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHeaderValue"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public BasicAuthenticationHeaderValue(string userName, string password)
            : base("Basic", EncodeCredential(userName, password))
        { }

        /// <summary>
        /// Encodes the credential.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">userName</exception>
        public static string EncodeCredential(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
            password ??= string.Empty;

            Encoding encoding = Encoding.UTF8;
            string credential = String.Format("{0}:{1}", userName, password);

            return Convert.ToBase64String(encoding.GetBytes(credential));
        }

    }
}