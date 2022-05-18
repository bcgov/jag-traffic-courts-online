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
using TrafficCourts.Citizen.Service.Mappings;
using TrafficCourts.Citizen.Service.Services.Impl;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Messaging;
using TicketStorageType = TrafficCourts.Citizen.Service.Configuration.TicketStorageType;
using FluentValidation.AspNetCore;

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

        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger, options =>
        {
            // add MassTransit source
            options
            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
            .AddRedisInstrumentation();
        });

        // Redis
        builder.AddRedis();

        // configure application 
        var configuration = builder.Configuration.Get<CitizenServiceConfiguration>();

        builder.Services.UseConfigurationValidation();
        builder.UseTicketSearch(logger);

        if (configuration.TicketStorage == TicketStorageType.InMemory)
        {
            builder.AddInMemoryFilePersistence();
        }
        else if (configuration.TicketStorage == TicketStorageType.ObjectStore)
        {
            builder.AddObjectStorageFilePersistence();
        }

        // Form Recognizer
        builder.Services.ConfigureValidatableSetting<FormRecognizerOptions>(builder.Configuration.GetSection(FormRecognizerOptions.Section));
        builder.Services.AddTransient<IFormRecognizerService, FormRecognizerService>();
        builder.Services.AddTransient<IFormRecognizerValidator, FormRecognizerValidator>();

        builder.Services.AddSingleton<ILookupService, RedisLookupService>();
        builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

        // MassTransit
        builder.Services.AddMassTransit(builder.Configuration, logger);

        // add MediatR handlers in this program
        builder.Services.AddMediatR(typeof(Startup).Assembly);

        // use lowercase routes
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        builder.Services.AddTransient<IConfigureOptions<JsonOptions>, ConfigureJsonOptions>();

        builder.Services.AddSingleton<IClock>(SystemClock.Instance);

        builder.Services.AddRecyclableMemoryStreams();

        // Registering and Initializing AutoMapper
        builder.Services.AddAutoMapper(typeof(NoticeOfDisputeToMessageContractMappingProfile));

        // Finds and registers all the fluent validators in the assembly
        builder.Services.AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Add services to the container.
        builder.Services
            .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
            .AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters());

        var swagger = SwaggerConfiguration.Get(builder.Configuration);

        if (swagger.Enabled)
        {
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

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }
    }
}
