using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using TrafficCourts.Common.Configuration;

namespace DisputeWorker
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
                config.AddConsumer<DisputeOrderedConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(rabbitBaseUri), hostConfig =>
                    {
                        hostConfig.Username(rabbitMqSettings.Username);
                        hostConfig.Password(rabbitMqSettings.Password);
                    });

                    cfg.ReceiveEndpoint($"DisputeOrdered_queue", endpoint =>
                    {
                        endpoint.Consumer<DisputeOrderedConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();
        }
    }
 
}
