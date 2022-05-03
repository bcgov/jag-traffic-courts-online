using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Features.Mail;
using TrafficCourts.Messaging;
using Serilog;
using TrafficCourts.Common.Configuration;

var builder = WebApplication.CreateBuilder(args);
var logger = GetLogger(builder);
builder.Configuration.AddVaultSecrets(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ArcApiConfiguration>(builder.Configuration.GetRequiredSection("ArcApiConfiguration"));
builder.Services.Configure<OracleDataApiConfiguration>(builder.Configuration.GetRequiredSection("OracleDataApiConfiguration"));
builder.Services.Configure<SmtpConfiguration>(builder.Configuration.GetRequiredSection("SmtpConfiguration"));
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetRequiredSection("EmailConfiguration"));

builder.Services.AddTransient<IOracleDataApiService, OracleDataApiService>();
builder.Services.AddTransient<ISubmitDisputeToArcService, SubmitDisputeToArcService>();
builder.Services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

void AddConsumers(IBusRegistrationConfigurator cfg)
{
    // TODO: use cfg.AddConsumers(params Type[] types) or cfg.AddConsumers(params Assembly[] assemblies)
    cfg.AddConsumer<SubmitDisputeConsumer>();
    cfg.AddConsumer<DisputeApprovedConsumer>();
    cfg.AddConsumer<SendEmailConsumer>();
}

builder.Services.AddMassTransit(builder.Configuration, logger, AddConsumers);

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


static Serilog.ILogger GetLogger(WebApplicationBuilder app)
{
    var configuration = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console();

    if (app.Environment.IsDevelopment())
    {
        configuration.WriteTo.Debug();
    }

    return configuration.CreateLogger();
}
