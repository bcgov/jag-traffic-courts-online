using Microsoft.AspNetCore.Server.Kestrel.Core;
using TrafficCourts.Ticket.Search.Service;
using TrafficCourts.Ticket.Search.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

builder.ConfigureServices();

builder.UseOpenShiftIntegration(_ => _.CertificateMountPoint = "/var/run/secrets/service-cert");

builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 25 * 1024 * 1024; // allow large transfers
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TicketSearchService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
