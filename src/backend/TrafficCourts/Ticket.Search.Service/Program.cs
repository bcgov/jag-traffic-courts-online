using TrafficCourts.Common.Configuration;
using TrafficCourts.Ticket.Search.Service.Configuration;
using TrafficCourts.Ticket.Search.Service;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TrafficCourts.Common.Health;

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilog<TicketSearchServiceConfiguration>();

// Add services to the container.
builder.ConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    // this endpoint returns HTTP 200 if all "liveness" checks have passed, otherwise, it returns HTTP 500
    endpoints.MapHealthChecks("/self", new HealthCheckOptions()
    {
        Predicate = registration => registration.Tags.Contains(HealthCheckType.Liveness)
    });

    // this endpoint returns HTTP 200 if all "readiness" checks have passed, otherwise, it returns HTTP 500
    endpoints.MapHealthChecks("/ready", new HealthCheckOptions()
    {
        Predicate = registration => registration.Tags.Contains(HealthCheckType.Readiness)
    });
});

app.Run();
