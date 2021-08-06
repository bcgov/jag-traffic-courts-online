using Microsoft.Extensions.DependencyInjection;

namespace Gov.CitizenApi.Features.Lookups.Configuration
{
    /// <summary>
    /// Extension to inject lookup service in service collection
    /// </summary>
    public static class LookupsServiceConfigurationExtension
    {
        public static void AddLookupsService(this IServiceCollection collection)
        {
            collection.AddSingleton<ILookupsService, LookupsService>();
        }
    }
}
