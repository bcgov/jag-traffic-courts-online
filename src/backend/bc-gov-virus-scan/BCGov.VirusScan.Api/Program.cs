using BCGov.VirusScan.Api.Monitoring;
using FastEndpoints;
using FastEndpoints.Swagger;
using NSwag;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddVirusScan();

builder.AddOpenTelemetry();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDoc(shortSchemaNames: true, tagIndex: 2);

builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = true;
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

// not sure if this is working yet
app.UseOpenTelemetryPrometheusScrapingEndpoint(PrometheusScraping.EndpointFilter);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
    app.UseSwaggerUI();
}

app.Run();
