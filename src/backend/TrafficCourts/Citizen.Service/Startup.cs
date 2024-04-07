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
using TrafficCourts.Caching;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging;
using FluentValidation;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using ZiggyCreatures.Caching.Fusion;
using TrafficCourts.Common.Health;

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
            options
                .AddFusionCacheInstrumentation(options =>
                {
                    // TODO: allow setting from config, useful in dev but not test/prod
                    options.IncludeMemoryLevel = false;
                    options.IncludeDistributedLevel = false;
                    options.IncludeBackplane = false;
                })
                .AddCitizenServiceInstrumentation()
                .AddComsClientInstrumentation()
                .AddMassTransitInstrumentation();
        },
            options =>
            {
                options
                    .AddFusionCacheInstrumentation()
                    .AddCitizenServiceInstrumentation()
                    .AddComsClientInstrumentation()
                    .AddMassTransitInstrumentation();
            }
        );

        builder.AddDefaultHealthChecks();

        builder.Services.AddHttpContextAccessor();

        // Redis
        var redisConnectionString = builder.AddRedis();

        builder.Services
            .AddFusionCache()
            .WithCacheKeyPrefix(Caching.Cache.Prefix)
            .WithCommonDistributedCacheOptions(redisConnectionString);

        // configure application 
        builder.Services.UseConfigurationValidation();
        builder.UseTicketSearch(logger);

        // Form Recognizer
        UseFormRecognizer(builder);

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

    private static void UseFormRecognizer(WebApplicationBuilder builder)
    {
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
