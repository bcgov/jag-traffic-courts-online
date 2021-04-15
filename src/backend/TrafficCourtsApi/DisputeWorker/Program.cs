using System;
using System.Configuration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DisputeWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    var rabbitMqSettings = configuration.GetSection("RabbitMq").Get<RabbitMqConfiguration>();
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<DisputeConsumer>();
                        x.AddBus(provider => ConfigureBus(provider, rabbitMqSettings));
                    });
                    services.AddMassTransitHostedService(true);
                    services.AddHostedService<Worker>();
                });

        private static IBusControl ConfigureBus(
            IServiceProvider provider,
            RabbitMqConfiguration rabbitMqConfigurations) => Bus.Factory.CreateUsingRabbitMq(
            cfg =>
            {
                cfg.Host(
                    rabbitMqConfigurations.Host,
                    "/",
                    hst =>
                    {
                        hst.Username(rabbitMqConfigurations.Username);
                        hst.Password(rabbitMqConfigurations.Password);
                    });

                cfg.ReceiveEndpoint($"{typeof(Dispute).Namespace}.{typeof(Dispute).Name}", endpoint =>
                {
                    endpoint.Consumer<DisputeConsumer>(provider);
                });

            });
    }

    public class RabbitMqConfiguration
    {
        public RabbitMqConfiguration()
        {
            this.Host = "localhost";
            this.Port = 5672;
            this.Username = "guest";
            this.Password = "guest";
        }

        /// <summary>
        /// RabbitMq Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// RabbitMq Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// RabbitMq Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// RabbitMq Password
        /// </summary>
        public string Password { get; set; }
    }
}
