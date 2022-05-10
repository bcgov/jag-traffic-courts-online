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

            builder.Services.UseConfigurationValidation();

            builder.Services.ConfigureValidatableSetting<SftpConnectionOptions>(builder.Configuration.GetRequiredSection(SftpConnectionOptions.Section));
            builder.Services.ConfigureValidatableSetting<SftpOptions>(builder.Configuration.GetRequiredSection(SftpOptions.Section));

            builder.Services.AddTransient<ISftpService, SftpService>();

            builder.Services.AddTransient<SftpClient>(services =>
            {
                var connectionOptions = services.GetRequiredService<SftpConnectionOptions>();
                var sshPrivateKeyPath = connectionOptions.SshPrivateKeyPath;

                // configuration validates that if SshPrivateKeyPath is not null or empty, then the file must exist
                if (!string.IsNullOrEmpty(sshPrivateKeyPath))
                {
                    var privateKey = new PrivateKeyFile(sshPrivateKeyPath);
                    return new SftpClient(connectionOptions.Host, connectionOptions.Port, connectionOptions.Username, new[] { privateKey });
                }

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
