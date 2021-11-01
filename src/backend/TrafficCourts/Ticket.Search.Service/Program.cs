using TrafficCourts.Common.Configuration;
using TrafficCourts.Ticket.Search.Service.Configuration;
using TrafficCourts.Ticket.Search.Service.Services.Authentication;

using MediatR;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilog<TicketSearchServiceConfiguration>();
builder.UseAuthenticationClient();

// Add services to the container.

builder.Services.AddMediatR(typeof(Program));
builder.Services.AddMemoryCache();

builder.Services.AddOpenTelemetryMetrics();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
