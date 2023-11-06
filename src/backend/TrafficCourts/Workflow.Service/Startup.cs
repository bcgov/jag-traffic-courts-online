using MassTransit;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;
using System.Reflection;
using TrafficCourts.Arc.Dispute.Client;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.VirusScan;
using TrafficCourts.Messaging;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Sagas;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Services.EmailTemplates;

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
        }, meters: new string[] { "MassTransit", "ComsClient", "WorkflowService" });

        builder.Services.AddControllers();

        AddSwagger(builder, assembly, logger);

        builder.Services.AddSingleton<IClock>(SystemClock.Instance);
        builder.Services.ConfigureValidatableSetting<EmailConfiguration>(builder.Configuration.GetRequiredSection(EmailConfiguration.Section));
        builder.Services.ConfigureValidatableSetting<SmtpConfiguration>(builder.Configuration.GetRequiredSection(SmtpConfiguration.Section));

        builder.Services.AddHashids(builder.Configuration);
        builder.Services.AddEmailVerificationTokens();
        builder.AddRedis();

        builder.Services.AddTransient<IVerificationEmailTemplate, VerificationEmailTemplate>();

        builder.Services.AddOracleDataApiClient(builder.Configuration);

        builder.Services.AddTransient<IOracleDataApiService, OracleDataApiService>();

        // add the Arc Dispute Client
        builder.Services.AddArcDisputeClient(builder.Configuration, section: "ArcApiConfiguration");

        // Add COMS (Object Management Service) Client
        builder.Services.AddObjectManagementService("COMS");

        // add the Virus Scan Client
        builder.Services.AddVirusScanClient(builder.Configuration);

        builder.Services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
        builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();
        builder.Services.AddTransient<IFileHistoryService, FileHistoryService>();
        builder.Services.AddTransient<IWorkflowDocumentService, WorkflowDocumentService>();

        builder.Services.AddEmailTemplates();

        // Configure Entity Framework Context for VerifyEmailAddressStateDbContext

        var connectionString = GetConnectionString(builder.Configuration, "Saga");
        builder.Services.AddDbContext<VerifyEmailAddressStateDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.MigrationsAssembly(assembly.GetName().Name);
                options.MigrationsHistoryTable($"__{nameof(VerifyEmailAddressStateDbContext)}");

                // TODO: validate these, came from example https://github.com/MassTransit/Sample-Outbox/blob/master/src/Sample.Api/Program.cs
                options.EnableRetryOnFailure(5);
                options.MinBatchSize(1);
            });
        });

        // add hosted service before masstransit so the db migration will run before masstransit does
        builder.Services.AddHostedService<DatabaseMigrationHostedService<VerifyEmailAddressStateDbContext>>();

        builder.Services.AddMassTransit(Diagnostics.Source.Name, builder.Configuration, logger, config =>
        {
            config.AddConsumers(assembly);

            RedisOptions redis = new RedisOptions();
            var section = builder.Configuration.GetSection(RedisOptions.Section);
            section.Bind(redis);

            config.AddSagaStateMachine<VerifyEmailAddressStateMachine, VerifyEmailAddressState, VerifyEmailAddressSagaDefinition>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    r.AddDbContext<DbContext, VerifyEmailAddressStateDbContext>();
                });

            config.AddSagas(assembly);
        });

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

    private static string GetConnectionString(ConfigurationManager configuration, string name)
    {
        // dev environments, my just use connection string section with name
        // but when using Crunchy Postgres, the generated secrets do not 
        // have a compatible connection string, so we bind host, port, database, username and password
        // from a section based on the name
        string? connectionString = configuration.GetConnectionString(name);
        if (connectionString is null)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            var sectionName = $"{name}DbConnectionString";
            var section = configuration.GetSection(sectionName);
            section.Bind(builder);

            connectionString = builder.ConnectionString;
        }

        return connectionString;
    }
}
