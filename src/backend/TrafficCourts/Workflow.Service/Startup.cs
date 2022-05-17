using MassTransit;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Features.Mail;
using TrafficCourts.Messaging;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Workflow.Service.Mappings;

namespace TrafficCourts.Workflow.Service;

public static class Startup
{
    public static void ConfigureApplication(this WebApplicationBuilder builder, Serilog.ILogger logger)
    {
        // Add services to the container.
        builder.AddSerilog();
        builder.AddOpenTelemetry(Diagnostics.Source, logger, options =>
        {
            options.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
        });

        builder.Services.AddControllers();

        var swagger = SwaggerConfiguration.Get(builder.Configuration);
        if (swagger.Enabled)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        builder.Services.Configure<ArcApiConfiguration>(builder.Configuration.GetRequiredSection("ArcApiConfiguration"));
        builder.Services.Configure<OracleDataApiConfiguration>(builder.Configuration.GetRequiredSection(OracleDataApiConfiguration.Section));
        builder.Services.ConfigureValidatableSetting<EmailConfiguration>(builder.Configuration.GetSection(EmailConfiguration.Section));
        builder.Services.ConfigureValidatableSetting<SmtpConfiguration>(builder.Configuration.GetRequiredSection(SmtpConfiguration.Section));

        builder.Services.AddTransient<IOracleDataApiService, OracleDataApiService>();
        builder.Services.AddTransient<ISubmitDisputeToArcService, SubmitDisputeToArcService>();
        builder.Services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
        builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

        void AddConsumers(IBusRegistrationConfigurator cfg)
        {
            cfg.AddConsumers(typeof(Startup).Assembly); // add all the consumers in this assembly
        }

        builder.Services.AddMassTransit(builder.Configuration, logger, AddConsumers);

        // Registering and Initializing AutoMapper
        builder.Services.AddAutoMapper(typeof(MessageContractToNoticeOfDisputeMappingProfile));

    }
}
