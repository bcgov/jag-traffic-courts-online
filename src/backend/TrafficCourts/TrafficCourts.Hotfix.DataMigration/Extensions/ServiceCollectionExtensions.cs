using TrafficCourts.Hotfix.DataMigration.Hotfixes;

namespace TrafficCourts.Hotfix.DataMigration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all hotfix services with the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddHotfixServices(this IServiceCollection services)
        {
            // Register individual hotfix services
            services.AddScoped<IHotfix, Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix>();
            
            // Add more hotfix services here as they are created
            // services.AddScoped<IHotfix, AnotherHotfixService>();
            // services.AddScoped<IHotfix, YetAnotherHotfixService>();

            // Register the hotfix manager service
            services.AddScoped<IHotfixManager, HotfixManager>();

            return services;
        }
    }
}
