using TrafficCourts.Citizen.Service;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Configuration.Validation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Serilog.ILogger logger = builder.GetProgramLogger();

builder.ConfigureApplication(logger); // this can throw ConfigurationErrorsException

var app = builder.Build();

// Configure the HTTP request pipeline.
var swagger = SwaggerConfiguration.Get(builder.Configuration);
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
