using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Messaging.Configuration;
using TrafficCourts.Messaging.MessageContracts;
using Xunit;

namespace TrafficCourts.Messaging.Test
{
    public class RabbitMqTest
    {
        [Fact]
        public async Task publish_works()
        {
            IServiceProvider serviceProvider = CreateServiceProvider(
                configureBusRegistration: (configure) =>
                {
                    //configure.AddConsumers(....);
                },
                configureBusFactory: (configure) => {
                });

            var busControl = serviceProvider.GetRequiredService<IBusControl>();
            //busControl.Start();

            await busControl.Publish(new DisputeApproved());

            await Task.Delay(5000);
        }

        private IServiceProvider CreateServiceProvider(
            Action<IBusRegistrationConfigurator>? configureBusRegistration = null, 
            Action<IBusFactoryConfigurator>? configureBusFactory = null)
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = GetRabbitMqConfiguration();

            // configure mass transit
            BusConfiguratorExtensions.AddMassTransit(services, configuration, Serilog.Log.Logger, configureBusRegistration, configureBusFactory);

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        private IConfiguration GetRabbitMqConfiguration()
        {
            var values = new Dictionary<string, string>
            {
                { $"{BusConfiguratorExtensions.MassTransitSection}:Transport", MassTransitTransport.RabbitMq.ToString() }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();

            return configuration;
        }
    }

    public abstract class Consumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        private int _consumeCount = 0;

        public int ConsumeCount => _consumeCount;

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            Interlocked.Increment(ref _consumeCount);
            await ConsumeCore(context);
        }

        protected virtual Task ConsumeCore(ConsumeContext<TMessage> context)
        {
            return Task.CompletedTask;
        }
    }

    public class Ping
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UtcNow;
    }

    public class Pong
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UtcNow;
    }

    public class PingConsumer : Consumer<Ping>
    {
        protected override Task ConsumeCore(ConsumeContext<Ping> context)
        {
            context.Publish<Pong>(new Pong { Now = context.Message.Now });

            return base.ConsumeCore(context);
        }
    }

    public class PingConsumerDefinition : ConsumerDefinition<PingConsumer>
    {

    }


    public class PongConsumer : Consumer<Pong>
    {
        protected override Task ConsumeCore(ConsumeContext<Pong> context)
        {
            return base.ConsumeCore(context);
        }
    }
}