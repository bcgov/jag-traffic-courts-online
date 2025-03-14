using Microsoft.Extensions.Configuration;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text;
using TrafficCourts.OrdsDataService;
using TrafficCourts.OrdsDataService.Justin;
using TrafficCourts.OrdsDataService.Occam;
using TrafficCourts.OrdsDataService.Tco;

namespace Microsoft.Extensions.DependencyInjection;

public static class OrdsDataServiceExtensions
{
    public class OrdsDataServiceOptions
    {
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public static void AddOrdsDataService(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO-DKAY split these options by TCO vs OCCAM so we can init both
        var options = new OrdsDataServiceOptions();
        configuration.GetSection("OrdsDataService").Bind(options);


        services.AddMemoryCache();

        services.AddHttpClient<TcoOrdsDataServiceClient>(client =>
        {
            client.BaseAddress = new Uri(options.Address);
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(options.Username, options.Password);
        })
        .AddHttpMessageHandler(sp =>
        {
            var cache = sp.GetRequiredService<Caching.Memory.IMemoryCache>();
            var metrics = sp.GetRequiredService<IOrdsDataServiceOperationMetrics>();
            ETagHandler handler = new ETagHandler(cache, metrics);
            return handler;
        });

        // TODO-DKAY How do we make sure this is being cached separately from the TCO stuff? In theory the endpoints should manage that, but...?
        services.AddHttpClient<OccamOrdsDataServiceClient>(client =>
        {
            client.BaseAddress = new Uri(options.Address);
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(options.Username, options.Password);
        })
        .AddHttpMessageHandler(sp =>
        {
            var cache = sp.GetRequiredService<Caching.Memory.IMemoryCache>();
            var metrics = sp.GetRequiredService<IOrdsDataServiceOperationMetrics>();
            ETagHandler handler = new ETagHandler(cache, metrics);
            return handler;
        });

        services.AddSingleton<IOrdsDataServiceOperationMetrics, OrdsDataServiceOperationMetrics>();

        // justin
        services.AddTransient<IAgencyRepository, AgencyRepository>();
        services.AddTransient<ICityRepository, CityRepository>();
        services.AddTransient<ICountryRepository, CountryRepository>();
        services.AddTransient<ILanguageRepository, LanguageRepository>();
        services.AddTransient<IProvinceRepository, ProvinceRepository>();
        services.AddTransient<IStatuteRepository, StatuteRepository>();

        // tco
        services.AddTransient<IAuditLogEntryTypeRepository, AuditLogEntryTypeRepository>();
        services.AddTransient<IDisputeStatusTypeRepository, DisputeStatusTypeRepository>();
        services.AddTransient<IDisputeCaseFileSummaryRepository, DisputeCaseFileSummaryRepository>();

        // occam
        services.AddTransient<IOccamDisputeRepository, OccamDisputeRepository>();
    }


    private sealed class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        public BasicAuthenticationHeaderValue(string username, string password)
            : base("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")))
        {
        }
    }
}
