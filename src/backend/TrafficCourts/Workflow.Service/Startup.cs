using MassTransit;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Features.Mail;
using TrafficCourts.Messaging;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Workflow.Service.Mappings;
using System.Reflection;

namespace TrafficCourts.Workflow.Service;

public static class Startup
{
    public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        // this assembly, used in a couple locations below for registering things
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Add services to the container.
        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger, options =>
        {
            options.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
        });

        builder.Services.AddControllers();

        AddSwagger(builder, assembly, logger);

        builder.Services.Configure<ArcApiConfiguration>(builder.Configuration.GetRequiredSection("ArcApiConfiguration"));
        builder.Services.Configure<OracleDataApiConfiguration>(builder.Configuration.GetRequiredSection(OracleDataApiConfiguration.Section));
        builder.Services.ConfigureValidatableSetting<EmailConfiguration>(builder.Configuration.GetSection(EmailConfiguration.Section));
        builder.Services.ConfigureValidatableSetting<SmtpConfiguration>(builder.Configuration.GetRequiredSection(SmtpConfiguration.Section));

        builder.Services.AddTransient<IOracleDataApiService, OracleDataApiService>();
        builder.Services.AddTransient<ISubmitDisputeToArcService, SubmitDisputeToArcService>();
        builder.Services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
        builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

        builder.Services.AddMassTransit(Diagnostics.Source.Name, builder.Configuration, logger, config => config.AddConsumers(assembly));

        builder.Services.AddAutoMapper(assembly); // Registering and Initializing AutoMapper
    }

    /// <summary>
    /// Adds swagger if enabled bu configuration
    /// </summary>
    private static void AddSwagger(WebApplicationBuilder builder, Assembly assembly, Serilog.ILogger logger)
    {
        var swagger = SwaggerConfiguration.Get(builder.Configuration);
        if (swagger.Enabled)
        {
            logger.Information("Swagger is enabled");

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }
    }
}