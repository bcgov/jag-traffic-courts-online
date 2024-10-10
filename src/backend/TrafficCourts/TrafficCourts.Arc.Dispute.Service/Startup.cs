using Renci.SshNet;
using Serilog;
using System.Reflection;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Services;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Arc.Dispute.Service;

public static class Startup
{
    public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // this assembly, used in a couple locations below for registering things
        Assembly assembly = Assembly.GetExecutingAssembly();

        builder.Services.AddSingleton(TimeProvider.System);

        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger);

        // Registering and Initializing AutoMapper
        builder.Services.AddAutoMapper(assembly);
        builder.Services.AddRecyclableMemoryStreams();

        builder.Services.AddScoped<IArcFileService, ArcFileService>();

        builder.Services.UseConfigurationValidation();

        builder.Services.ConfigureValidatableSetting<SftpConnectionOptions>(builder.Configuration.GetRequiredSection(SftpConnectionOptions.Section));
        builder.Services.ConfigureValidatableSetting<SftpOptions>(builder.Configuration.GetRequiredSection(SftpOptions.Section));

        builder.Services.AddScoped<ISftpService, SftpService>();

        builder.Services.AddTransient<SftpClient>(serviceProvider =>
        {
            var connectionOptions = serviceProvider.GetRequiredService<SftpConnectionOptions>();

            // try to use ssh private key to authenticate
            PrivateKeyFile? privateKey = connectionOptions.GetPrivateKey();
            if (privateKey is not null)
            {
                return new SftpClient(connectionOptions.Host, connectionOptions.Port, connectionOptions.Username, new[] { privateKey });
            }

            // use username and password
            return new SftpClient(connectionOptions.Host, connectionOptions.Port, connectionOptions.Username, connectionOptions.Password);
        });

        // Add services to the container.
        builder.Services
            .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
            .AddJsonOptions(options =>
            {
                options.UseDateOnlyTimeOnlyStringConverters();
                options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
            });

        AddSwagger(builder, assembly, logger);
    }

    /// <summary>
    /// Adds swagger if enabled by configuration
    /// </summary>
    private static void AddSwagger(WebApplicationBuilder builder, Assembly assembly, Serilog.ILogger logger)
    {
        // determine if swagger is enabled or not
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
                    Title = "Traffic Court Online ARC Api",
                    Description = "An API sending dispute data to ARC",
                });

                // Commented out these lines for now since the .xml file is not found and prevents the api's swagger-ui to be accessed
                //var xmlFilename = $"{assembly.GetName().Name}.xml";
                //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }
    }
}
