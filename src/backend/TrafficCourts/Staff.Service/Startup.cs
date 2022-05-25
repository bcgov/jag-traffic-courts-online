using MassTransit;
using MediatR;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Messaging;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Configuration;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service;

public static class Startup
{
    public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        // this assembly, used in a couple locations below for registering things
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Add services to the container.
        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger, options =>
        {
            options
            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
        });

        // Redis
        builder.AddRedis();

        builder.Services.AddMassTransit(Diagnostics.Source.Name, builder.Configuration, logger);

        // Render enums as strings rather than ints
        builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Ensure routes (api endpoints) are lowercase
        //   The RFC 3986 specifications denote that URIs care case-sensitive. Per best practices for rest apis, route endpoints
        //   should be lowercase to avoid confusion about inconsistent capitalisation.
        builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        builder.Services.AddAuthentication(builder.Configuration);

        builder.Services.AddFilePersistence(builder.Configuration);

        // Add DisputeService
        builder.Services.ConfigureValidatableSetting<OracleDataApiConfiguration>(builder.Configuration.GetRequiredSection(OracleDataApiConfiguration.Section));
        builder.Services.AddSingleton<IDisputeService, DisputeService>();

        builder.Services.AddSingleton<ILookupService, RedisLookupService>();

        builder.Services.AddMediatR(assembly);

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
