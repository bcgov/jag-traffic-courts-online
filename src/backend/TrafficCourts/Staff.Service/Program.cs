using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using System.Reflection;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Logging;

var builder = WebApplication.CreateBuilder(args);
var logger = GetLogger(builder);

// Add services to the container.

AddOpenTelemetry(builder, logger);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => {
    loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers());
});

builder.Services.AddControllers();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var swagger = SwaggerConfiguration.Get(builder.Configuration);

if (swagger.Enabled)
{
    logger.Information("Swagger is enabled");
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Gets a logger for application setup.
/// </summary>
/// <returns></returns>
static Serilog.ILogger GetLogger(WebApplicationBuilder builder)
{
    var configuration = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console();

    if (builder.Environment.IsDevelopment())
    {
        configuration.WriteTo.Debug();
    }

    return configuration.CreateLogger();
}

static void AddOpenTelemetry(WebApplicationBuilder builder, Serilog.ILogger logger)
{
    string? endpoint = builder.Configuration["OTEL_EXPORTER_JAEGER_ENDPOINT"];

    if (string.IsNullOrEmpty(endpoint))
    {
        logger.Information("Jaeger endpoint is not configured, no telemetry will be collected.");
        return;
    }

    var resourceBuilder = ResourceBuilder.CreateDefault().AddService(Diagnostics.ServiceName, serviceInstanceId: Environment.MachineName);

    builder.Services.AddOpenTelemetryTracing(options =>
    {
        options
            .SetResourceBuilder(resourceBuilder)
            .AddHttpClientInstrumentation(options =>
            {
                    // do not trace calls to splunk
                    options.Filter = (message) => message.RequestUri?.Host != "hec.monitoring.ag.gov.bc.ca";

            })
            .AddAspNetCoreInstrumentation()
            .AddSource(Diagnostics.Source.Name)
            .AddJaegerExporter();
    });
}
