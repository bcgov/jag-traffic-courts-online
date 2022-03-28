using Calzolari.Grpc.AspNetCore.Validation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Refit;
using Serilog;
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
using TrafficCourts.Ticket.Search.Service.Logging;
using TrafficCourts.Ticket.Search.Service.Services;
using TrafficCourts.Ticket.Search.Service.Validators;

namespace TrafficCourts.Ticket.Search.Service
{
    public static class Startup
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            Serilog.ILogger logger = GetLogger();

            builder.Configuration.Add<TicketSearchServiceConfigurationProvider>();

            logger.Information("Configuring Serilog logging");

            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
            });

            var configuration = builder.Configuration.Get<TicketSearchServiceConfiguration>();

            if (configuration == null || configuration.TicketSearch == null || !configuration.TicketSearch.IsValid())
            {
                const string message = "Ticket Search configuration is not configured correctly, check the TicketSearch section";
                logger.Error(message);
                throw new ConfigurationErrorsException(message);
            }

            builder.Services.AddSingleton(configuration.TicketSearch);

            //builder.Services.AddOpenTelemetryMetrics();
            logger.Information("Adding health checks");
            AddHealthChecks(builder, configuration);

            AddOpenTelemetry(builder, logger);

            if (configuration.TicketSearch.SearchType == TicketSearchType.RoadSafety)
            {
                logger.Information("Using RSI ticket search service");
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
                logger.Information("Using mock ticket search service");

                builder.Services.AddTransient<ITicketSearchService, MockTicketSearchService>();

                // if a valid data file has been configured, use that, otherwise use the embedded data
                if (FileMockDataProvider.HasValidConfiguration(builder.Configuration))
                {
                    logger.Information("Using file mock ticket data");
                    builder.Services.AddTransient<IMockDataProvider, FileMockDataProvider>();
                }
                else
                {
                    logger.Information("Using embedded mock ticket data");
                    builder.Services.AddTransient<IMockDataProvider, EmbeddedMockDataProvider>();
                }
            }

            // Add services to the container.
            logger.Information("Configuring services");
            builder.Services.AddGrpc(options =>
            {
                options.EnableMessageValidation();
            });

            builder.Services.AddGrpcValidation();
            builder.Services.AddValidator<SearchRequestValidator>();

            builder.Services.AddMemoryCache();

            logger.Information("Adding OpenShift integration");

            builder.UseOpenShiftIntegration(_ => _.CertificateMountPoint = "/var/run/secrets/service-cert");

            builder.WebHost.UseKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 25 * 1024 * 1024; // allow large transfers
            });
        }

        private static void AddHealthChecks(WebApplicationBuilder builder, TicketSearchServiceConfiguration configuration)
        {
            IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { HealthCheckType.Liveness });

            if (configuration?.TicketSearch?.SearchType == TicketSearchType.RoadSafety)
            {
                healthChecksBuilder.AddCheck<AccessTokenAvailableHealthCheck>("access-token-available", failureStatus: HealthStatus.Unhealthy, tags: new[] { HealthCheckType.Readiness });
            }
        }

        private static void AddOpenTelemetry(WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            string? endpoint = builder.Configuration["OTEL_EXPORTER_JAEGER_ENDPOINT"];

            if (string.IsNullOrEmpty(endpoint))
            {
                logger.Information("Jaeger endpoint is not configured, no telemetry will be collected.");
                return;
            }

            logger.Information("Adding OpenTelemetry");

            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(Diagnostics.ServiceName, serviceInstanceId: Environment.MachineName);

            builder.Services.AddOpenTelemetryTracing(options =>
            {
                options
                    .SetResourceBuilder(resourceBuilder)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddSource(Diagnostics.Source.Name)
                    .AddJaegerExporter();
            });
        }

        /// <summary>
        /// Gets a logger for application setup.
        /// </summary>
        /// <returns></returns>
        private static Serilog.ILogger GetLogger()
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();

            return logger;
        }
    }
}
