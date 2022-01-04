using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Refit;
using System.Configuration;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Health;
using TrafficCourts.Ticket.Search.Service.Configuration;
using TrafficCourts.Ticket.Search.Service.Features.Search;
using TrafficCourts.Ticket.Search.Service.Features.Search.MockTicketSearch;
using TrafficCourts.Ticket.Search.Service.Features.Search.RoadSafetyTicketSearch;
using TrafficCourts.Ticket.Search.Service.Health;
using TrafficCourts.Ticket.Search.Service.Services;
using TrafficCourts.Ticket.Search.Service.Services.Authentication;

namespace TrafficCourts.Ticket.Search.Service
{
    public static class Startup
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Configuration.Add<TicketSearchServiceConfigurationProvider>();
            var configuration = builder.Configuration.Get<TicketSearchServiceConfiguration>();

            if (configuration == null || configuration.TicketSearch == null || !configuration.TicketSearch.IsValid())
            {
                const string message = "Ticket Search configuration is not configured correctly, check the TicketSearch section";
                Serilog.Log.Logger.Error(message);
                throw new ConfigurationErrorsException(message);
            }

            AddHealthChecks(builder, configuration);

            if (configuration.TicketSearch.SearchType == TicketSearchType.RoadSafety)
            {
                builder.Services.AddHttpClient<AuthenticationClient>(client =>
                {
                    client.BaseAddress = configuration.TicketSearch.AuthenticationUrl.BaseAddress();
                    client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                    client.DefaultRequestHeaders.Add("return-client-request-id", "true");
                });

                builder.Services
                    .AddRefitClient<IRoadSafetyTicketSearchApi>()
                    .ConfigureHttpClient(c => c.BaseAddress = configuration.TicketSearch.ResourceUrl.BaseAddress())
                    .AddHttpMessageHandler<AuthenticationHandler>();

                builder.Services.AddTransient<AuthenticationHandler>();
                builder.Services.AddTransient<IAuthenticationClient, AuthenticationClient>();

                builder.Services.AddTransient<ITicketSearchService, RoadSafetyTicketSearchService>();
                builder.Services.AddTransient<ITokenCache, TokenCache>();

                builder.Services.AddHostedService<AccessTokenUpdateWorker>();
            }
            else
            {
                builder.Services.AddTransient<ITicketSearchService, MockTicketSearchService>();
            }


            builder.Services.AddSingleton(configuration.TicketSearch);

            builder.Services.AddMediatR(typeof(Program));

            builder.Services.AddMemoryCache();

            builder.Services.AddOpenTelemetryMetrics();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        private static void AddHealthChecks(WebApplicationBuilder builder, TicketSearchServiceConfiguration configuration)
        {
            IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { HealthCheckType.Liveness });

            if (configuration.TicketSearch.SearchType == TicketSearchType.RoadSafety)
            {
                healthChecksBuilder.AddCheck<AccessTokenAvailableHealthCheck>("access-token-available", failureStatus: HealthStatus.Unhealthy, tags: new[] { HealthCheckType.Readiness })
                    .AddSslHealthCheck(options =>
                    {
                        var host = configuration.TicketSearch.AuthenticationUrl.Host;
                        var port = configuration.TicketSearch.AuthenticationUrl.Port;

                        options.AddHost(host, port, 0);
                    }, "authentication-url-valid", tags: new[] { HealthCheckType.Readiness })
                    .AddSslHealthCheck(options =>
                    {
                        var host = configuration.TicketSearch.ResourceUrl.Host;
                        var port = configuration.TicketSearch.ResourceUrl.Port;

                        options.AddHost(host, port, 0);
                    }, "resource-url-valid", tags: new[] { HealthCheckType.Readiness })
                    ;
            }

        }
    }
}
