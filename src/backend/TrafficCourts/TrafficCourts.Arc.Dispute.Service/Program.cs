using Microsoft.Extensions.Options;
using Renci.SshNet;
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

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Startup>();

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
        logger.LogInformation(exception, "SSH key for the provided path has not been found. Using default password authentication.");
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
