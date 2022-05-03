using Microsoft.Extensions.Options;
using Renci.SshNet;
using Serilog;
using System.Text;
using System.Text.Json;
using TrafficCourts.Arc.Dispute.Service;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Services;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Converters;

var builder = WebApplication.CreateBuilder(args);
Serilog.ILogger logger = GetLogger(builder);

builder.Configuration.AddVaultSecrets(logger);

// determine if swagger is enabled or not
var swagger = SwaggerConfiguration.Get(builder.Configuration);

// Add services to the container.

builder.Services
    .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
    .AddJsonOptions(options =>
    {
        options.UseDateOnlyTimeOnlyStringConverters();
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    });

if (swagger.Enabled)
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static Serilog.ILogger GetLogger(WebApplicationBuilder app)
{
    var configuration = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console();

    if (app.Environment.IsDevelopment())
    {
        configuration.WriteTo.Debug();
    }

    return configuration.CreateLogger();
}
