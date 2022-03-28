using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IOracleInterfaceService, OracleInterfaceService>();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<SubmitDisputeConsumer>();
    cfg.UsingRabbitMq((context, configurator) =>
    {
        var configuration = context.GetService<IConfiguration>();
        var rabbitMqConfig = configuration.GetSection(nameof(RabbitMqConfig)).Get<RabbitMqConfig>();
        var retryConfiguration = configuration.GetSection(nameof(RetryConfiguration)).Get<RetryConfiguration>();

        configurator.Host(rabbitMqConfig.Host);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(false));
        configurator.UseConcurrencyLimit(retryConfiguration.ConcurrencyLimit);
        configurator.UseMessageRetry(r =>
        {
            r.Ignore<ArgumentNullException>();
            r.Ignore<InvalidOperationException>();
            r.Interval(retryConfiguration.RetryTimes, TimeSpan.FromMinutes(retryConfiguration.RetryInterval));
        });
    });

});

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
