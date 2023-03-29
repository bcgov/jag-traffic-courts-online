using BCGov.VirusScan.Api.Controllers;
using BCGov.VirusScan.Api.Monitoring;
using System.Reflection;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Exceptions.Core;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    var destructuringOptionsBuilder = new DestructuringOptionsBuilder();
    var destructurers = destructuringOptionsBuilder
        .WithDefaultDestructurers();

    loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.WithExceptionDetails(destructurers)
        .Enrich.WithEnvironmentVariable("HOSTNAME", "MachineName")
        .Enrich.WithEnvironmentVariable("OPENSHIFT_BUILD_COMMIT", "GIT_SHA");
});

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<VirusScanController>()); // some anchor class
builder.Services.AddVirusScan();
builder.AddInstrumentation();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

//builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.MapControllers();

// not sure if this is working yet
app.UseOpenTelemetryPrometheusScrapingEndpoint(PrometheusScraping.EndpointFilter);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
