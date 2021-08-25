using Gov.TicketWorker.Features.Disputes;
using Gov.TicketWorker.Features.Notifications;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using TrafficCourts.Common.Configuration;
using TrafficCourts.Common.Contract;

namespace Gov.TicketWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = SerilogLogging.GetDefaultLogger<Program>();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseSerilog(SplunkEventCollector.Configure)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    ConfigureServiceBus(services, configuration);
                    services.AddHostedService<Worker>();

                });

        internal static void ConfigureServiceBus(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQConfiguration>(configuration.GetSection("RabbitMq"));

            var rabbitMqSettings = configuration.GetSection("RabbitMq").Get<RabbitMQConfiguration>();
            var rabbitBaseUri = $"amqp://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}";

            services.AddMassTransit(config =>
            {
                config.AddConsumer<DisputeRequestedConsumer>();
                config.AddConsumer<DisputeUpdatedConsumer>();
                config.AddConsumer<NotificationRequestedConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(rabbitBaseUri), hostConfig =>
                    {
                        hostConfig.Username(rabbitMqSettings.Username);
                        hostConfig.Password(rabbitMqSettings.Password);
                    });

                    cfg.ReceiveEndpoint( (typeof(DisputeContract)).GetQueueName(), endpoint =>
                    {
                        endpoint.Consumer<DisputeRequestedConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint(Constants.DisputeUpdatedQueueName, endpoint =>
                    {
                        endpoint.Consumer<DisputeUpdatedConsumer>(ctx);
                    });

                    cfg.ReceiveEndpoint((typeof(NotificationContract)).GetQueueName(), endpoint =>
                    {
                        endpoint.Consumer<NotificationRequestedConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();
        }
    }
 
}
