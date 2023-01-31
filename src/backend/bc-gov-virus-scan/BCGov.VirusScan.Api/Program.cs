using BCGov.VirusScan.Api.Controllers;
using BCGov.VirusScan.Api.Models;
using BCGov.VirusScan.Api.Monitoring;
using MediatR;
using Microsoft.AspNetCore.Builder;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(VirusScanController)); // some anchor class
builder.Services.AddVirusScan();
builder.AddOpenTelemetry();

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
