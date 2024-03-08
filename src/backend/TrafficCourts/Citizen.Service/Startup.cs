using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Serilog;
using System.Configuration;
using System.Reflection;
using TrafficCourts.Citizen.Service.Authentication;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Services.Impl;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging;
using FluentValidation;
using Microsoft.OpenApi.Models;
using TrafficCourts.Citizen.Service.Caching;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi;

namespace TrafficCourts.Citizen.Service;

public static class Startup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
    /// <exception cref="ConfigurationErrorsException"></exception>
    public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton(TimeProvider.System);

        // this assembly, used in a couple locations below for registering things
        Assembly assembly = Assembly.GetExecutingAssembly();

        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger, options =>
        {
            options.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddRedisInstrumentation();
        }, meters: new string[] { "MassTransit", "ComsClient", "CitizenService" });

        builder.Services.AddHttpContextAccessor();

        builder.AddCaching();

        // configure application 
        builder.Services.UseConfigurationValidation();
        builder.UseTicketSearch(logger);

        // Form Recognizer
        builder.Services.ConfigureValidatableSetting<FormRecognizerOptions>(builder.Configuration.GetSection(FormRecognizerOptions.Section));
        string apiVersion = FormRecognizerOptions.Get(builder.Configuration).ApiVersion ?? String.Empty;
        if (FormRecognizerOptions.v2_1 == apiVersion)
        {
            // API version 2.1, an older, but working version for deployment to OpenShift 
            builder.Services.AddTransient<IFormRecognizerService, FormRecognizerService_2_1>();
        }
        else if (FormRecognizerOptions.v2022_08_31 == apiVersion)
        {
            // API version 2022_08_31 is the latest version available for deployment to OpenShift
            builder.Services.AddTransient<IFormRecognizerService, FormRecognizerService_2022_08_31>();
        }
        else
        {
            throw new ArgumentException($"Unknown Form Recognizer ApiVersion '{apiVersion}'. Must be one of '2.1' or '2022-08-31'.");
        }

        builder.Services.AddTransient<IFormRecognizerValidator, FormRecognizerValidator>();

        builder.Services.AddLanguageLookup();
        builder.Services.AddStatuteLookup();
        builder.Services.AddAgencyLookup();
        builder.Services.AddProvinceLookup();
        builder.Services.AddTransient<IRedisCacheService, RedisCacheService>();
        builder.Services.AddTransient<ICitizenDocumentService, CitizenDocumentService>();

        // Add COMS (Object Management Service) Client
        builder.Services.AddObjectManagementService("COMS");

        // MassTransit
        builder.Services.AddMassTransit(Diagnostics.Source.Name, builder.Configuration, logger);

        // add MediatR handlers in this program
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // use lowercase routes
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        builder.Services.ConfigureValidatableSetting<OAuthOptions>(builder.Configuration.GetRequiredSection(OAuthOptions.Section));
        builder.Services.AddTransient<IOAuthUserService, OAuthUserService>()
            .AddHttpClient<IOAuthUserService, OAuthUserService>();

        builder.Services.AddAuthentication(builder.Configuration);

        builder.Services.AddAuthorization();

        builder.Services.AddTransient<IConfigureOptions<JsonOptions>, ConfigureJsonOptions>();

        // simple reversible hashing for passing information back and forth to client using salt from parameters
        builder.Services.AddHashids(builder.Configuration);

        builder.Services.AddEmailVerificationTokens();

        builder.Services.AddRecyclableMemoryStreams();

        builder.Services.AddAutoMapper(assembly); // Registering and Initializing AutoMapper

        // Finds and registers all the fluent validators in the assembly
        builder.Services.AddValidatorsFromAssembly(assembly);

        // Add services to the container.
        builder.Services
            .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
            .AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters());

        AddSwagger(builder, assembly, logger);
    }

    /// <summary>
    /// Enables redis caching and registers named Fusion Cache instances.
    /// </summary>
    /// <param name="builder"></param>
    private static void AddCaching(this WebApplicationBuilder builder)
    {
        // Redis
        var connectionString = builder.AddRedis();

        builder.Services
<<<<<<< HEAD
            .AddFusionCache(Cache.TicketSearch.Name)
=======
            .AddFusionCache(Cache.TicketSearch)
>>>>>>> 445037a74 (Add hybrid ticket search, fix ticket search error responses)
            .WithCommonFusionCacheOptions(connectionString);
    }

    private static IFusionCacheBuilder WithCommonFusionCacheOptions(this IFusionCacheBuilder builder, string connectionString)
    {
        // values below come from the step by step guide
        // https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/StepByStep.md#8-backplane-more
<<<<<<< HEAD
        // note: the cache duration is based on the result set
=======
>>>>>>> 445037a74 (Add hybrid ticket search, fix ticket search error responses)
        builder.WithOptions(options =>
            {
                options.DistributedCacheCircuitBreakerDuration = TimeSpan.FromSeconds(2);
            })
            .WithDefaultEntryOptions(new FusionCacheEntryOptions
            {
<<<<<<< HEAD
=======
                Duration = TimeSpan.FromDays(1),
>>>>>>> 445037a74 (Add hybrid ticket search, fix ticket search error responses)
                // fail safe
                IsFailSafeEnabled = true,
                FailSafeMaxDuration = TimeSpan.FromHours(2),
                FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
                // facotry
                FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
                FactoryHardTimeout = TimeSpan.FromMilliseconds(1500),
                // distributed cache
                DistributedCacheSoftTimeout = TimeSpan.FromSeconds(1),
                DistributedCacheHardTimeout = TimeSpan.FromSeconds(2),
                AllowBackgroundDistributedCacheOperations = true,

                JitterMaxDuration = TimeSpan.FromSeconds(2)
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(new RedisCache(new RedisCacheOptions() { Configuration = connectionString }))
            .WithBackplane(new RedisBackplane(new RedisBackplaneOptions() { Configuration = connectionString }))
            ;

        return builder;
    }

    /// <summary>
    /// Adds swagger if enabled bu configuration
    /// </summary>
    private static void AddSwagger(WebApplicationBuilder builder, Assembly assembly, Serilog.ILogger logger)
    {
        var swagger = SwaggerConfiguration.Get(builder.Configuration);

        if (swagger.Enabled)
        {
            logger.Information("Swagger is enabled");

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Traffic Court Online Citizen Api",
                    Description = "An API for creating violation ticket disputes",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                });

                options.EnableAnnotations();
                options.UseDateOnlyTimeOnlyStringConverters();

                var xmlFilename = $"{assembly.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }
    }
}
