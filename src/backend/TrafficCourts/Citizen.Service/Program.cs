using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using MediatR;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Messaging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit<CitizenServiceConfiguration>(builder);

builder.Services.AddMediatR(typeof(Program).GetType().Assembly);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton(services =>
{
    var backendUrl = builder.Configuration["TicketSearchUrl"];

    var channel = GrpcChannel.ForAddress(backendUrl, new GrpcChannelOptions
    {
        Credentials = ChannelCredentials.Insecure,
        ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } },
        ServiceProvider = services
    });

    return channel;
});

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
