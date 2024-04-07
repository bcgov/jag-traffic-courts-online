using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Health;
using TrafficCourts.Configuration.Validation;
using TrafficCourts.Diagnostics;
using TrafficCourts.Workflow.Service;

var builder = WebApplication.CreateBuilder(args);
var logger = builder.GetProgramLogger();

builder.ConfigureApplication(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
var swagger = SwaggerConfiguration.Get(builder.Configuration);
if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint(PrometheusScraping.EndpointFilter);
app.MapDefaultHealthCheckEndpoints();

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
catch (Exception exception)
{
    logger.Fatal(exception, "Exception occurred causing application termination");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
