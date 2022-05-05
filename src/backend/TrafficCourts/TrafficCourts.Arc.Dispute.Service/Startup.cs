using Microsoft.Extensions.Options;
using Renci.SshNet;
using Serilog;
using System.Reflection;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Services;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Arc.Dispute.Service
{
    public static class Startup
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.AddSerilog();
            builder.AddOpenTelemetry(Diagnostics.Source, logger);

            // Registering and Initializing AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddRecyclableMemoryStreams();

            builder.Services.AddTransient<IArcFileService, ArcFileService>();

            builder.Services.Configure<SftpConfig>(builder.Configuration.GetRequiredSection(SftpConfig.Section));

            builder.Services.AddTransient<ISftpService, SftpService>();

            builder.Services.AddTransient<SftpClient>(services =>
            {
                var configurationOptions = services.GetRequiredService<IOptions<SftpConfig>>();
                var configuration = configurationOptions.Value;
                try
                {
                    var sshPrivateKeyPath = configuration.SshPrivateKeyPath;

                    if (!string.IsNullOrEmpty(sshPrivateKeyPath) && File.Exists(sshPrivateKeyPath))
                    {
                        var privateKey = new PrivateKeyFile(sshPrivateKeyPath);
                        return new SftpClient(configuration.Host, configuration.Port, configuration.Username, new[] { privateKey });
                    }

                    return new SftpClient(configuration.Host, configuration.Port, configuration.Username, configuration.Password);
                }
                catch (System.IO.DirectoryNotFoundException exception)
                {
                    logger.Information(exception, "SSH key for the provided path has not been found. Using default password authentication.");
                    return new SftpClient(configuration.Host, configuration.Port, configuration.Username, configuration.Password);
                }
            });


            // Add services to the container.
            builder.Services
                .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
                .AddJsonOptions(options =>
                {
                    options.UseDateOnlyTimeOnlyStringConverters();
                    options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                });


            // determine if swagger is enabled or not
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
                        Title = "Traffic Court Online ARC Api",
                        Description = "An API sending dispute data to ARC",
                    });

                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                });
            }
        }
    }
}
