using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TrafficCourts.Common.Features.Lookups
{
    public static class Extensions
    {
        /// <summary>
        /// Adds statute lookup. Callers must ensure Redis is registered.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddStatuteLookup(this IServiceCollection services)
        {
            services.AddTransient<IStatuteLookupService, StatuteLookupService>();
            services.AddMemoryCache();

            services.AddTransient<IRequestHandler<StatuteLookup.Request, StatuteLookup.Response>, StatuteLookup.Handler>();

            return services;
        }
    }
}
