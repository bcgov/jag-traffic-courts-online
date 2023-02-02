using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Configuration.Validation;
using TrafficCourts.Common.Diagnostics;
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

app.UseHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("ready")
});

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
    logger.Fatal(exception, "Exception occured causing application termination");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
