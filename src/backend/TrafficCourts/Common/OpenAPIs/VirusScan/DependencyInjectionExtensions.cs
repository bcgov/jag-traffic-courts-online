using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Common.Configuration.Validation;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;

namespace TrafficCourts.Common.OpenAPIs.VirusScan;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds <see cref="IVirusScanClient"/> as an injectable service. Requires configuring the VirusScan:BaseUrl value.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="SettingsValidationException">The &quot;VirusScan&quot; configuration section is not found.</exception>
    public static IHttpClientBuilder AddVirusScanClient(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        IConfigurationSection section = configuration.GetSection(VirusScanApiConfiguration.Section);
        if (!section.Exists())
        {
            throw SettingsValidationException.RequiredSectionNotFound(VirusScanApiConfiguration.Section);
        }

        services.ConfigureValidatableSetting<VirusScanApiConfiguration>(section);

        IHttpClientBuilder builder = services.AddHttpClient<IVirusScanClient, VirusScanClient>((services, client) =>
        {
            var configuration = services.GetRequiredService<VirusScanApiConfiguration>();
            client.BaseAddress = new Uri(configuration.BaseUrl);
        });

        return builder;
    }
}
