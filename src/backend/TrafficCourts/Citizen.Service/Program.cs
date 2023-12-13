using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TrafficCourts.Citizen.Service;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Configuration.Validation;
using TrafficCourts.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Serilog.ILogger logger = builder.GetProgramLogger();

builder.ConfigureApplication(logger); // this can throw ConfigurationErrorsException

var app = builder.Build();

AddMockInvoiceEndpoint(app);

// Configure the HTTP request pipeline.
var swagger = SwaggerConfiguration.Get(builder.Configuration);
if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseOpenTelemetryPrometheusScrapingEndpoint(PrometheusScraping.EndpointFilter);

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


void AddMockInvoiceEndpoint(WebApplication application)
{
    var type = builder.Configuration.GetSection($"TicketSearch:SearchType").Get<TicketSearchType>();
    if (type == TicketSearchType.Mock)
    {
        application.MapPost("/api/invoice", [AllowAnonymous] (Invoice invoice, [FromServices] IMemoryCache cache) =>
        {
            if (string.IsNullOrEmpty(invoice?.InvoiceNumber))
            {
                return Results.BadRequest(new { error = "Invoice number missing" });
            }

            MockInvoiceCache invoiceCache = new(cache);
            invoiceCache.Add(invoice);

            return Results.Ok();
        });
    }
}