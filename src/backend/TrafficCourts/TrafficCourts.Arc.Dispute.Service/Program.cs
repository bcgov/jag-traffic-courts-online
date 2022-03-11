using Microsoft.Extensions.Options;
using Renci.SshNet;
using TrafficCourts.Arc.Dispute.Service;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Registering and Initializing AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

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
        var privateKey = new PrivateKeyFile(configuration.SshPrivateKeyPath);
        if (privateKey != null)
        {
            return new SftpClient(configuration.Host, configuration.Port == 0 ? 22 : configuration.Port, configuration.Username, new[] { privateKey });
        }
        else
        {
            return new SftpClient(configuration.Host, configuration.Port == 0 ? 22 : configuration.Port, configuration.Username, configuration.Password);
        }
    }
    catch (System.IO.DirectoryNotFoundException exception)
    {
        logger.LogInformation(exception, "SSH key for the provided path has not been found. Using default password authentication.");
        return new SftpClient(configuration.Host, configuration.Port == 0 ? 22 : configuration.Port, configuration.Username, configuration.Password);
    }    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
