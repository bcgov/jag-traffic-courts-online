using FastEndpoints;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Configuration.Validation;
using TrafficCourts.Diagnostics;
using TrafficCourts.Staff.Service;

var builder = WebApplication.CreateBuilder(args);
var logger = builder.GetProgramLogger();

builder.ConfigureApplication(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting(); //if using, this call must go before auth/cors/fastendpoints middleware

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c => { });

app.UseEndpoints(c => { });

app.MapControllers()
    .RequireAuthorization(); // This will set a default policy that says a user has to be authenticated

app.UseOpenTelemetryPrometheusScrapingEndpoint(PrometheusScraping.EndpointFilter);

var swagger = SwaggerConfiguration.Get(builder.Configuration);
if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
