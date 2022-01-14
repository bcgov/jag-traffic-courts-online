using Microsoft.AspNetCore.Server.Kestrel.Core;
using TrafficCourts.Ticket.Search.Service;
using TrafficCourts.Ticket.Search.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

builder.ConfigureServices();
builder.WebHost.UseKestrel(options =>
{

    options.ListenAnyIP(8080, o =>
    {
        o.Protocols = HttpProtocols.Http2;

        // OpenShift
        //   CertificateMountPoint -> /var/run/secrets/service-cert
        //     tls.crt
        //     tls.key
        //
        // Local
        //o.UseHttpsWithFullChain("d:\\temp\\tls.crt", "d:\\temp\\tls.key");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TicketSearchService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
