using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Consumers;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<SubmitDisputeConsumer>();
    cfg.UsingRabbitMq((context, configurator) =>
    {
        var configuration = context.GetService<IConfiguration>();
        var rabbitMqConfig = configuration.GetSection(nameof(RabbitMqConfig)).Get<RabbitMqConfig>();
        configurator.Host(rabbitMqConfig.Host);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(false));
    });

});

builder.Services.AddMassTransitHostedService();

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

app.UseHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = (check) => check.Tags.Contains("ready")
});

app.Run();
