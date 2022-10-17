using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NodaTime;
using OpenTelemetry.Trace;
using Serilog;
using System.Configuration;
using System.Reflection;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Services.Impl;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging;
using FluentValidation.AspNetCore;
using TrafficCourts.Common.Features.FilePersistence;
using HashidsNet;

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

        // this assembly, used in a couple locations below for registering things
        Assembly assembly = Assembly.GetExecutingAssembly();

        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger, options =>
        {
            // add MassTransit source
            options.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddRedisInstrumentation();
        });

        // Redis
        builder.AddRedis();

        // configure application 
        builder.Services.UseConfigurationValidation();
        builder.UseTicketSearch(logger);

        builder.Services.AddFilePersistence(builder.Configuration);

        // Form Recognizer
        builder.Services.ConfigureValidatableSetting<FormRecognizerOptions>(builder.Configuration.GetSection(FormRecognizerOptions.Section));
        string apiVersion = FormRecognizerOptions.Get(builder.Configuration).ApiVersion ?? String.Empty;
        if (FormRecognizerOptions.v2_1 == apiVersion)
        {
            // API version 2.1 is the latest version available for deployment to OpenShift
            builder.Services.AddTransient<IFormRecognizerService, FormRecognizerService_2_1>();
        }
        else if (FormRecognizerOptions.v2022_06_30_preview == apiVersion)
        {
            // API version 2022_06_30_preview is the latest version used by Azure cloud services, but this version is not containerizable yet
            builder.Services.AddTransient<IFormRecognizerService, FormRecognizerService_2022_06_30_preview>();
        }
        else
        {
            throw new ArgumentException($"Unknown Form Recognizer ApiVersion '{apiVersion}'. Must be one of '2.1' or '2022-06-30-preview'.");
        }

        builder.Services.AddTransient<IFormRecognizerValidator, FormRecognizerValidator>();

        builder.Services.AddStatuteLookup();
        builder.Services.AddTransient<IRedisCacheService, RedisCacheService>();

        // MassTransit
        builder.Services.AddMassTransit(Diagnostics.Source.Name, builder.Configuration, logger);

        // add MediatR handlers in this program
        builder.Services.AddMediatR(assembly);

        // use lowercase routes
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        builder.Services.AddTransient<IConfigureOptions<JsonOptions>, ConfigureJsonOptions>();

        // simple reversible hashing for passing information back and forth to client using salt from parameters
        builder.Services.AddHashids(builder.Configuration);
        builder.Services.AddEmailVerificationTokens();

        builder.Services.AddSingleton<IClock>(SystemClock.Instance);

        builder.Services.AddRecyclableMemoryStreams();
        
        builder.Services.AddAutoMapper(assembly); // Registering and Initializing AutoMapper
        
        builder.Services.AddFluentValidation(configure =>
        {
            // Finds and registers all the fluent validators in the assembly
            configure.RegisterValidatorsFromAssembly(assembly);
        });

        // Add services to the container.
        builder.Services
            .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
            .AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters());

        AddSwagger(builder, assembly, logger);
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

                options.UseDateOnlyTimeOnlyStringConverters();

                var xmlFilename = $"{assembly.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }
    }
}
