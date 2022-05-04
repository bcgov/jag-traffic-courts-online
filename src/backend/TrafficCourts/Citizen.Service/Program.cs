using Serilog;
using System.ComponentModel;
using System.Reflection;
using TrafficCourts.Citizen.Service;
using TrafficCourts.Citizen.Service.Mappings;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Configuration.Validation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Serilog.ILogger logger = GetLogger(builder);

builder.ConfigureApplication(logger); // this can throw ConfigurationErrorsException

// Registering and Initializing AutoMapper
builder.Services.AddAutoMapper(typeof(NoticeOfDisputeToMessageContractMappingProfile));

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
        //options.CustomSchemaIds(x => x.FullName); // TODO: remove this, causes problems with generated swagger
    });
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();


bool isDevelopment = app.Environment.IsDevelopment();
try
{
    app.Run();
}
catch (SettingsValidationException exception)
{
    logger.Fatal(exception, "Configuration error");

    if (isDevelopment)
    {        
        throw; // see the error in the IDE
    }
}

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
