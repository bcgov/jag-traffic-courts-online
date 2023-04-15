using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Features.Lookups;

namespace TrafficCourts.Common;

[ExcludeFromCodeCoverage]
public static partial class Extensions
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

    /// <summary>
    /// Adds language lookup. Callers must ensure Redis is registered.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLanguageLookup(this IServiceCollection services)
    {
        services.AddTransient<ILanguageLookupService, LanguageLookupService>();
        services.AddMemoryCache();

        services.AddTransient<IRequestHandler<LanguageLookup.Request, LanguageLookup.Response>, LanguageLookup.Handler>();

        return services;
    }

    /// <summary>
    /// Adds agency lookup. Callers must ensure Redis is registered.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAgencyLookup(this IServiceCollection services)
    {
        services.AddTransient<IAgencyLookupService, AgencyLookupService>();
        services.AddMemoryCache();

        services.AddTransient<IRequestHandler<AgencyLookup.Request, AgencyLookup.Response>, AgencyLookup.Handler>();

        return services;
    }

    /// <summary>
    /// Adds province lookup. Callers must ensure Redis is registered.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddProvinceLookup(this IServiceCollection services)
    {
        services.AddTransient<IProvinceLookupService, ProvinceLookupService>();
        services.AddMemoryCache();

        services.AddTransient<IRequestHandler<ProvinceLookup.Request, ProvinceLookup.Response>, ProvinceLookup.Handler>();

        return services;
    }
}
