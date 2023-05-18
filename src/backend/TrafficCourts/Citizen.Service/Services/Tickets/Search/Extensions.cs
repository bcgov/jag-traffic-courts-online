using Refit;
using System.Configuration;
using TrafficCourts.Citizen.Service.Services.Tickets.Search;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

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
            else
            {
                // this could happen if some one adds a new default member before Mock.
                logger.Fatal("Unknown ticket search type: {SearchType}", type);
                throw new ConfigurationErrorsException($"{Section}:SearchType is invalid. Value was {type}.");
            }
        }

        private static void UseRsiTickets(WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            logger.Information("Using RSI ticket search service");

            builder.Services.ConfigureValidatableSetting<RsiServiceOptions>(builder.Configuration.GetSection(Section));

            builder.Services.AddHttpClient<OpenIdAuthenticationClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<RsiServiceOptions>();
                var uri = options.AuthenticationUrl;
                client.BaseAddress = new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}");
                client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                client.DefaultRequestHeaders.Add("return-client-request-id", "true");
            });

            builder.Services
                .AddRefitClient<IRoadSafetyTicketSearchApi>()
                .ConfigureHttpClient((serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<RsiServiceOptions>();
                    var uri = options.ResourceUrl;
                    client.BaseAddress = new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}");
                })
                .AddHttpMessageHandler<AuthenticationHandler>();

            builder.Services.AddOptions();
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<ITokenCache, TokenCache>();

            builder.Services.AddTransient<AuthenticationHandler>();
            // seems the OpenId endpoint does not work and we need to get access token via basic auth now
            ////builder.Services.AddTransient<IAuthenticationClient, OpenIdAuthenticationClient>();
            builder.Services.AddTransient<IAuthenticationClient, BasicAuthAuthenticationClient>();

            builder.Services.AddTransient<ITicketInvoiceSearchService, RoadSafetyTicketSearchService>();
            builder.Services.AddHostedService<AccessTokenUpdateWorker>();
        }

        public static void UseMockTickets(WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            logger.Information("Using mock ticket search service");
            builder.Services.AddTransient<ITicketInvoiceSearchService, MockTicketSearchService>();

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
    }
}
