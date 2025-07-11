using Microsoft.Extensions.Configuration;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Text;
using TrafficCourts.OrdsDataService;
using TrafficCourts.OrdsDataService.Generated.OCCAM.Client.V1;
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
        var options_tco = new OrdsDataServiceOptions();
        configuration.GetSection("OrdsDataService_Tco").Bind(options_tco);

        services.AddMemoryCache();

        services.AddHttpClient<TcoOrdsDataServiceClient>(client =>
        {
            client.BaseAddress = new Uri(options_tco.Address);
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(options_tco.Username, options_tco.Password);
        })
        .AddHttpMessageHandler(sp =>
        {
            var cache = sp.GetRequiredService<Caching.Memory.IMemoryCache>();
            var metrics = sp.GetRequiredService<IOrdsDataServiceOperationMetrics>();
            ETagHandler handler = new ETagHandler(cache, metrics);
            return handler;
        });

        var options_occam = new OrdsDataServiceOptions();
        configuration.GetSection("OrdsDataService_Occam").Bind(options_occam);

        services.AddHttpClient<OccamOrdsDataServiceClient>(client =>
        {
            client.BaseAddress = new Uri(options_occam.Address);
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(options_occam.Username, options_occam.Password);
        })
        .AddHttpMessageHandler(sp =>
        {
            var cache = sp.GetRequiredService<Caching.Memory.IMemoryCache>();
            var metrics = sp.GetRequiredService<IOrdsDataServiceOperationMetrics>();
            ETagHandler handler = new ETagHandler(cache, metrics);
            return handler;
        });

        services.AddScoped<IOCCAMORDSDataServiceClientV1, OCCAMORDSDataServiceClientV1>();

        services.AddHttpClient<OCCAMORDSDataServiceClientV1>(client =>
        {
            client.BaseAddress = new Uri(options_occam.Address);
            client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(options_occam.Username, options_occam.Password);
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
        services.AddTransient<IOccamDisputeWithUpdateRequestRepository, OccamDisputeWithUpdateRequestRepository>();
    }


    private sealed class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        public BasicAuthenticationHeaderValue(string username, string password)
            : base("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")))
        {
        }
    }
}
