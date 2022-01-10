using Calzolari.Grpc.AspNetCore.Validation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Refit;
using System.Configuration;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Health;
using TrafficCourts.Ticket.Search.Service.Authentication;
using TrafficCourts.Ticket.Search.Service.Configuration;
using TrafficCourts.Ticket.Search.Service.Features.Search;
using TrafficCourts.Ticket.Search.Service.Features.Search.Mock;
using TrafficCourts.Ticket.Search.Service.Features.Search.Rsi;
using TrafficCourts.Ticket.Search.Service.Health;
using TrafficCourts.Ticket.Search.Service.Validators;

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

            builder.Services.AddSingleton(configuration.TicketSearch);
            
            //builder.Services.AddOpenTelemetryMetrics();

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

                //builder.Services.AddHostedService<AccessTokenUpdateWorker>();

            }
            else
            {
                builder.Services.AddTransient<ITicketSearchService, MockTicketSearchService>();
            }

            // Add services to the container.
            builder.Services.AddGrpc(options =>
            {
                options.EnableMessageValidation();
            });

            builder.Services.AddGrpcValidation();
            builder.Services.AddValidator<SearchRequestValidator>();

            builder.Services.AddMemoryCache();

        }

        private static void AddHealthChecks(WebApplicationBuilder builder, TicketSearchServiceConfiguration configuration)
        {
            IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { HealthCheckType.Liveness });

            if (configuration.TicketSearch.SearchType == TicketSearchType.RoadSafety)
            {
                healthChecksBuilder.AddCheck<AccessTokenAvailableHealthCheck>("access-token-available", failureStatus: HealthStatus.Unhealthy, tags: new[] { HealthCheckType.Readiness });
            }
        }
    }
}
