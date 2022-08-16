using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TrafficCourts.Arc.Dispute.Client;

public static class Extensions
{
    public static IServiceCollection AddArcDisputeClient(this IServiceCollection services, IConfiguration configuration, string section)
    {
        services.Configure<Configuration>(configuration.GetRequiredSection(section));
        services.AddHttpClient<IArcDisputeClient, ArcDisputeClient>(ConfigureClient);

        return services;
    }

    private static void ConfigureClient(IServiceProvider services, HttpClient client)
    {
        Configuration configuration = services.GetRequiredService<IOptions<Configuration>>().Value;
        client.BaseAddress = configuration.Uri;
    }
}
