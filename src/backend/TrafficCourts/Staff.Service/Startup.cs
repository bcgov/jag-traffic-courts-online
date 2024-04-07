using MassTransit;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Reflection;
using System.Text.Json.Serialization;
using TrafficCourts.Caching;
using TrafficCourts.Common;
using TrafficCourts.Common.Authentication;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Health;
using TrafficCourts.Messaging;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Services;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Staff.Service;

public static class Startup
{
    public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        // this assembly, used in a couple locations below for registering things
        Assembly assembly = Assembly.GetExecutingAssembly();

        builder.Services.AddSingleton(TimeProvider.System);

        // Add services to the container.
        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger, options =>
        {
            options
                .AddComsClientInstrumentation()
                .AddFusionCacheInstrumentation(options =>
                {
                    // TODO: allow setting from config, useful in dev but not test/prod
                    options.IncludeMemoryLevel = false;
                    options.IncludeDistributedLevel = false;
                    options.IncludeBackplane = false;
                })
                .AddMassTransitInstrumentation()
                .AddOracleDataApiInstrumentation()
                .AddRedisInstrumentation();
        },
        options => 
        {
            options
                .AddComsClientInstrumentation()
                .AddFusionCacheInstrumentation()
                .AddMassTransitInstrumentation()
                .AddOracleDataApiInstrumentation();
        });

        builder.AddDefaultHealthChecks();

        var redisConnectionString = builder.AddRedis();

        builder.Services
            .AddFusionCache()
            .WithCacheKeyPrefix(Caching.Cache.Prefix)
            .WithCommonDistributedCacheOptions(redisConnectionString);

        builder.Services.AddTransient<UserIdentityProviderHandler>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddOracleDataApi(builder.Configuration, builder =>
        {
            builder.AddHttpMessageHandler<UserIdentityProviderHandler>();
        });

        builder.Services.AddRecyclableMemoryStreams();
        builder.Services.AddMassTransit(Diagnostics.Source.Name, builder.Configuration, logger);

        // Render enums as strings rather than ints
        builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Ensure routes (api endpoints) are lowercase
        //   The RFC 3986 specifications denote that URIs care case-sensitive. Per best practices for rest apis, route endpoints
        //   should be lowercase to avoid confusion about inconsistent capitalisation.
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        builder.Services.AddAuthentication(builder.Configuration);

        builder.Services.AddAuthorization(builder.Configuration);

        builder.Services.AddKeycloakAdminApiClient(builder.Configuration);
        builder.Services.AddTransient<IKeycloakService, KeycloakService>();

        // Add DisputeService
        builder.Services.AddTransient<IDisputeService, DisputeService>();
        builder.Services.AddTransient<IFileHistoryService, FileHistoryService>();
        builder.Services.AddTransient<IEmailHistoryService, EmailHistoryService>();
        builder.Services.AddTransient<IJJDisputeService, JJDisputeService>();
        builder.Services.AddTransient<IStaffDocumentService, StaffDocumentService>();

        //builder.Services.AddTransient<IDisputeLockService, DisputeLockService>(); can use this one for in memory
        builder.Services.AddTransient<IDisputeLockService, RedisDisputeLockService>();

        builder.Services.AddLanguageLookup();
        builder.Services.AddStatuteLookup();
        builder.Services.AddAgencyLookup();
        builder.Services.AddProvinceLookup();
        builder.Services.AddCountryLookup();

        // Add COMS (Object Management Service) Client
        builder.Services.AddObjectManagementService("COMS");

        // Add CDOGS (Common Document Generation Service) Client
        builder.Services.AddDocumentGenerationService("CDOGS");
        builder.Services.AddTransient<IPrintDigitalCaseFileService, PrintDigitalCaseFileService>();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        AddSwagger(builder, assembly, logger);
    }

    /// <summary>
    /// Adds swagger if enabled by configuration
    /// </summary>
    private static void AddSwagger(WebApplicationBuilder builder, Assembly assembly, Serilog.ILogger logger)
    {
        var swagger = SwaggerConfiguration.Get(builder.Configuration);
        if (swagger.Enabled)
        {
            logger.Information("Swagger is enabled");
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "VTC Staff API",
                    Description = "Violation Ticket Centre Staff API",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                });

                c.EnableAnnotations();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
