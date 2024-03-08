using Microsoft.Extensions.Caching.Memory;
using Refit;
using System.Configuration;
using TrafficCourts.Citizen.Service.Services.Tickets.Search;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;
using TrafficCourts.Configuration.Validation;
using TrafficCourts.Core.Http;
using TrafficCourts.Http;

namespace TrafficCourts.Citizen.Service
{
    public static class Extensions
    {
        public const string Section = "TicketSearch";

        public static void UseTicketSearch(this WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            var type = builder.Configuration.GetSection($"{Section}:SearchType").Get<TicketSearchType>();

            builder.Services.AddTransient<ITicketSearchService, TicketSearchService>();

            if (type == TicketSearchType.RoadSafety)
            {
                UseRsiTickets(builder, logger);
            }
            else if (type == TicketSearchType.Mock)
            {
                UseMockTickets(builder, logger);
            }
            else if (type == TicketSearchType.Hybrid)
            {
                UseHybridTickets(builder, logger);

            }
            else
            {
                // this could happen if some one adds a new default member before Mock.
                logger.Fatal("Unknown ticket search type: {SearchType}", type);
                throw new ConfigurationErrorsException($"{Section}:SearchType is invalid. Value was {type}.");
            }
        }

        private static void UseRsiTicketsDependencies(WebApplicationBuilder builder)
        {
            builder.Services
                .AddRefitClient<IRoadSafetyTicketSearchApi>()
                .ConfigureHttpClient(ConfigureClient)
                .AddHttpMessageHandler((serviceProvider) => CreateOidcDelegatingHandler(serviceProvider, "TicketSearch"))
                .AddStandardResilienceHandler();

            builder.Services.AddOptions();
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<ITokenCache, TokenCache>();

            builder.Services.AddHostedService<RsiTokenRefreshService>(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var cache = serviceProvider.GetRequiredService<ITokenCache>();
                var options = GetConfiguration(serviceProvider, "TicketSearch");
                var logger = serviceProvider.GetRequiredService<ILogger<RsiTokenRefreshService>>();

                return new RsiTokenRefreshService(factory, "RSI", TimeProvider.System, cache, options, logger);
            });

        }
        private static void UseRsiTickets(WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            logger.Information("Using RSI ticket search service");
            UseRsiTicketsDependencies(builder);

            builder.Services.AddTransient<ITicketInvoiceSearchService, RoadSafetyTicketSearchService>();
        }

        public static void UseMockTicketDependencies(WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            builder.Services.AddMemoryCache();

            // determine if configured to use a external file or embedded data
            var section = builder.Configuration.GetSection(Section);
            FileMockDataSettings? settings = section.Get<FileMockDataSettings>();

            // if a valid data file has been configured, use that, otherwise use the embedded data
            if (!string.IsNullOrEmpty(settings?.MockDataPath))
            {
                logger.Information("Using file mock ticket data");
                builder.Services.ConfigureValidatableSetting<FileMockDataSettings>(section);
                builder.Services.AddTransient<IMockDataProvider, FileMockDataProvider>();
            }
            else
            {
                logger.Information("Using embedded mock ticket data");
                builder.Services.AddTransient<IMockDataProvider, EmbeddedMockDataProvider>();
            }
        }

        public static void UseMockTickets(WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            logger.Information("Using mock ticket search service");
            builder.Services.AddTransient<ITicketInvoiceSearchService, MockTicketSearchService>();
            UseMockTicketDependencies(builder, logger);
        }

        public static void UseHybridTickets(WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            logger.Information("Using hybrid ticket search service");

            // setup RSI
            UseRsiTicketsDependencies(builder);
            builder.Services.AddTransient<RoadSafetyTicketSearchService>();

            // setup mock
            UseMockTicketDependencies(builder, logger);
            builder.Services.AddTransient<MockTicketSearchService>();

            // use hybrid search
            builder.Services.AddTransient<ITicketInvoiceSearchService, HybridMockTicketSearchService>();
        }

        private static void ConfigureClient(IServiceProvider serviceProvider, HttpClient client)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            IConfigurationSection section = configuration.GetSection("TicketSearch");
            var resourceUrl = section["ResourceUrl"];

            if (string.IsNullOrEmpty(resourceUrl) || !Uri.TryCreate(resourceUrl, UriKind.Absolute, out var baseAddress))
            {
                throw new SettingsValidationException("TicketSearch", "ResourceUrl", " is required");
            }

            client.BaseAddress = baseAddress;
        }

        private static OidcConfidentialClientDelegatingHandler CreateOidcDelegatingHandler(IServiceProvider serviceProvider, string sectionName)
        {
            var configuration = GetConfiguration(serviceProvider, sectionName);
            var tokenCache = serviceProvider.GetRequiredService<ITokenCache>();
            var logger = serviceProvider.GetRequiredService<ILogger<OidcConfidentialClientDelegatingHandler>>();

            var handler = new OidcConfidentialClientDelegatingHandler(configuration, tokenCache, logger);
            return handler;
        }

        private static OidcConfidentialClientConfiguration GetConfiguration(IServiceProvider serviceProvider, string sectionName)
        {
            // we are not using ConfigureValidatableSetting because there may be multiple instances of OIDC clients
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            IConfigurationSection section = configuration.GetSection(sectionName);
            var oidc = new OidcConfidentialClientConfiguration();
            section.Bind(oidc);

            // validate
            if (string.IsNullOrEmpty(oidc.ClientId)) throw new SettingsValidationException(sectionName, nameof(OidcConfidentialClientConfiguration.ClientId), "is required");
            if (string.IsNullOrEmpty(oidc.ClientSecret)) throw new SettingsValidationException(sectionName, nameof(OidcConfidentialClientConfiguration.ClientSecret), "is required");
            if (oidc.TokenEndpoint is null)
            {
                // old configuration had this property on "AuthenticationUrl" so see if that is there
                string? authenticationUrl = section["AuthenticationUrl"];
                if (string.IsNullOrEmpty(authenticationUrl) || !Uri.TryCreate(authenticationUrl, UriKind.Absolute, out var tokenEndpoint)) 
                {
                    throw new SettingsValidationException(sectionName, nameof(OidcConfidentialClientConfiguration.TokenEndpoint), "is required");
                }

                // log out the warning that the deprecated configuration is still being used
                var logger = serviceProvider.GetRequiredService<ILogger<OidcConfidentialClientConfiguration>>();
                logger.LogWarning("OIDC configuration for {Section} is using deprecated configuration property AuthenticationUrl, please change to TokenEndpoint", section);

                oidc.TokenEndpoint = tokenEndpoint;
            }

            return oidc;
        }
    }
}
