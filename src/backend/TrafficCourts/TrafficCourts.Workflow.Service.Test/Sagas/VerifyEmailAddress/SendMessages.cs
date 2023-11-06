using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Test.Sagas.VerifyEmailAddress;

public class SendMessages
{
    [Fact(Skip = "Used to manually send a message to rabbitmq")]
    public async Task RunAsync()
    {
        IServiceProvider services = GetServiceProvider();

        IBus bus = services.GetRequiredService<IBus>();

        await bus.Publish(new RequestEmailVerification
        {
            NoticeOfDisputeGuid = Guid.NewGuid(),
            EmailAddress = "john.doe@example.com",
            TicketNumber = "AB12345678"
        });
    }

    private IServiceProvider GetServiceProvider()
    {
        ServiceCollection services = new ServiceCollection();

        Dictionary<string, string?> environmentVariables = new Dictionary<string, string?>()
        {
            { "MassTransit:Transport", "RabbitMq" },
            { "RabbitMq:Host", "localhost" },
            { "RabbitMq:Port", "5672" },
            { "RabbitMq:Username", "guest" },
            { "RabbitMq:Password", "guest" },
            { "Redis:ConnectionString", "redis:6379,password=password" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(environmentVariables)
            .Build();

        services.AddMassTransit(Diagnostics.Source.Name, configuration, Serilog.Log.Logger);

        return services.BuildServiceProvider();
    }
}
