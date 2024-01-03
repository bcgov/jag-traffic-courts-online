using Divergic.Logging.Xunit;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Services.EmailTemplates;
using Xunit.Abstractions;

namespace TrafficCourts.Workflow.Service.Test.Sagas.VerifyEmailAddress;

public class SagaTest
{
    private readonly Containers _containers = new Containers();
    private readonly ITestOutputHelper _output;

    public SagaTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(Skip = "Testing using containers, dont run normally")]
    public async Task Run()
    {
        ServiceProvider serviceProvider = await StartAsync();

        var bus = serviceProvider.GetRequiredService<IBus>();

        await bus.Publish<RequestEmailVerification>(new RequestEmailVerification
        {
            DisputeId = 0,
            EmailAddress = "philbolduc@gmail.com",
            TicketNumber = "AA12345678",
            NoticeOfDisputeGuid = Guid.NewGuid(),
            IsUpdateEmailVerification = false
        });

        await Task.Delay(TimeSpan.FromMinutes(5));

        Assert.True(true);
    }

    private async Task<ServiceProvider> StartAsync()
    {
        await _containers.StartAsync();

        ServiceCollection services = new ServiceCollection();

        services.AddLogging(config => 
        {
            config.SetMinimumLevel(LogLevel.Debug);
            config.AddProvider(new TestOutputLoggerProvider(_output));
        });

        var verificationEmailTemplate = Substitute.For<IVerificationEmailTemplate>();
        verificationEmailTemplate.Create(Arg.Any<SendEmailVerificationEmail>()).Returns(new EmailMessage());

        var emailSenderService = Substitute.For<IEmailSenderService>();
        var oracleDataApiService = Substitute.For<IOracleDataApiService>();

        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddSingleton(verificationEmailTemplate);
        services.AddSingleton(emailSenderService);
        services.AddSingleton(oracleDataApiService);


        Dictionary<string, string?> environmentVariables = new Dictionary<string, string?>()
        {
            { "MassTransit:Transport", "RabbitMq" },
            { "RabbitMq:Host", _containers.RabbitMq.Host },
            { "RabbitMq:Port", _containers.RabbitMq.Port.ToString() },
            { "RabbitMq:Username", "rabbitmq" },
            { "RabbitMq:Password", "rabbitmq" },
            { "Redis:ConnectionString", _containers.Redis.ConnectionString }
        };

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SagaTest>()
            .AddInMemoryCollection(environmentVariables)
            .Build();

        //services.AddSingleton<IConfiguration>(configuration);

        var workflowServiceAssembly = typeof(Startup).Assembly;
        services.AddMassTransit(nameof(SagaTest), configuration, Serilog.Log.Logger, config =>
        {
            config.AddConsumers(workflowServiceAssembly);

            RedisOptions redis = new RedisOptions();
            var section = configuration.GetSection(RedisOptions.Section);
            section.Bind(redis);

            //config.AddSagaStateMachine<VerifyEmailAddressStateMachine, VerifyEmailAddressSagaState, VerifyEmailAddressSagaDefinition>()
            //    .RedisRepository(redis.ConnectionString);

            config.AddSagas(workflowServiceAssembly);
        });


        //// TODO: use our extension method to register 
        //services.AddMassTransit(x =>
        //{
        //    x.AddSagaStateMachine<VerifyEmailAddressStateMachine, VerifyEmailAddressSagaState, VerifyEmailAddressSagaDefinition>()
        //        .RedisRepository(_containers.Redis.ConnectionString);
        //    x.AddSagas(workflowServiceAssembly);
        //    x.SetKebabCaseEndpointNameFormatter();
        //    x.AddConsumers(workflowServiceAssembly);
        //    x.UsingRabbitMq((context, cfg) =>
        //    {
        //        cfg.MessageTopology.SetEntityNameFormatter(new Messaging.PrefixEntityNameFormatter("Messages"));
        //        cfg.SendTopology.ErrorQueueNameFormatter = new ErrorQueueNameFormatter();
        //        cfg.SendTopology.DeadLetterQueueNameFormatter = new DeadLetterQueueNameFormatter();
        //        cfg.Host(_containers.RabbitMq.Host, _containers.RabbitMq.Port, "/", "docker", host =>
        //        {
        //            host.Username("rabbitmq");
        //            host.Password("rabbitmq");
        //        });
        //        cfg.ConfigureJsonSerializerOptions(settings =>
        //        {
        //            settings.Converters.Add(new DateOnlyJsonConverter());
        //            settings.Converters.Add(new TimeOnlyJsonConverter());
        //            settings.Converters.Add(new JsonStringEnumConverter());
        //            return settings;
        //        });
        //        cfg.ConfigureEndpoints(context);
        //    });
        //});

        ServiceProvider provider = services.BuildServiceProvider();

        // start masstransit's hosted service
        var hostedServices = provider.GetServices<IHostedService>().ToArray();

        foreach (var service in hostedServices)
            await service.StartAsync(CancellationToken.None).ConfigureAwait(false);

        return provider;
    }

    public class Containers
    {
        private readonly RabbitMqContainer _rabbitMq;
        private readonly PostgreSqlContainer _postgres;
        private readonly RedisContainer _redis;
        private readonly string _suffix = $"{Guid.NewGuid():n}"[..6];


        public Containers()
        {
            _rabbitMq = new RabbitMqBuilder()
                .WithImage("rabbitmq:3.11-management")
                .WithName($"rabbitmq-{_suffix}")
                .WithPortBinding(5672, true)
                .WithPortBinding(15672, true)
                .Build();

            RabbitMq = new RabbitMqConfiguration(_rabbitMq);

            _postgres = new PostgreSqlBuilder()
                .WithName($"postgres-{_suffix}")
                .WithPortBinding(5432, true)
                .Build();

            PostgreSql = new PostgreSqlConfiguration(_postgres);

            _redis = new RedisBuilder()
                .WithName($"redis-{_suffix}")
                .WithPortBinding(6379, true)
                .Build();

            Redis = new RedisConfiguration(_redis);
        }

        public async Task StartAsync()
        {
            await Task.WhenAll(_rabbitMq.StartAsync(), _postgres.StartAsync(), _redis.StartAsync());
        }

        public RabbitMqConfiguration RabbitMq { get; }
        public PostgreSqlConfiguration PostgreSql { get; }
        public RedisConfiguration Redis { get; }
    }
}


public class RabbitMqConfiguration : ContainerConfiguration<RabbitMqContainer>
{
    public RabbitMqConfiguration(RabbitMqContainer container) : base(container)
    {
    }
    public ushort Port => _container.GetMappedPublicPort(5672);
}

public class PostgreSqlConfiguration : ContainerConfiguration<PostgreSqlContainer>
{
    public PostgreSqlConfiguration(PostgreSqlContainer container) : base(container)
    {
    }
}

public class RedisConfiguration : ContainerConfiguration<RedisContainer>
{
    public RedisConfiguration(RedisContainer container) : base(container)
    {
    }

    public string ConnectionString => _container.GetConnectionString();
}

public abstract class ContainerConfiguration<TContainer> where TContainer : DockerContainer
{
    protected readonly TContainer _container;

    protected ContainerConfiguration(TContainer container)
    {
        _container = container;
    }

    /// <summary>
    /// Host will be the same as the name
    /// </summary>
    public string Host => _container.Name.TrimStart('/'); // name always gets leading slash

}
