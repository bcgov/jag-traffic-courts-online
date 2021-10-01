using Gov.TicketWorker.Features.Disputes;
using Gov.TicketWorker.Features.Emails;
using Gov.TicketWorker.Features.Notifications;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
                .UseSerilog(SplunkEventCollector.Configure)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    ConfigureServiceBus(services, configuration);
                    ConfigureEmailServices(services, configuration);
                    services.AddHostedService<Worker>();
                    
                });

        internal static void ConfigureEmailServices(IServiceCollection services, IConfiguration configuration)
        {
            var sender = configuration.GetSection("SMTPServer")["Sender"];
            var port = configuration.GetSection("SMTPServer")["Port"];
            var from = configuration.GetSection("Mail")["From"];
            var fromEmail = configuration.GetSection("Mail")["FromEmail"];
            string allowedEmails = configuration.GetSection("ALLOWED_RECIPIENT_EMAILS").Value;

            if (sender == null)
                Log.Logger.Error("SMTP Sender configuration not found for sending emails");
            if (fromEmail == null)
                Log.Logger.Error("FromEmail address configuration not found for sending emails");
            if (port == null)
                Log.Logger.Error("SMTP port number configuration not found for sending emails");

            if (sender != null && fromEmail != null && port != null)
            {
                if (allowedEmails != "*")
                {
                    var allowedEmailsList = allowedEmails?.Split(";").ToList();
                    services.AddSingleton<IEmailFilter>(new DefaultEmailFilter(allowedEmailsList ?? new List<string>()));
                }
                else
                {
                    services.AddSingleton<IEmailFilter>(new NotFilteredEmailFilter());
                }

                services.AddFluentEmail(fromEmail, from)
                      .AddLiquidRenderer()
                      .AddSmtpSender(new SmtpClient(sender)
                      {
                          EnableSsl = false,
                          Port = int.Parse(port),
                          DeliveryMethod = SmtpDeliveryMethod.Network
                      });
                services.AddScoped<IEmailSender, EmailSender>();
            }
        }

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

                    cfg.ReceiveEndpoint((typeof(InvalidContract<NotificationContract>)).GetQueueName(), endpoint => 
                    {
                        endpoint.Bind<InvalidContract<NotificationContract>>();
                    });
                });

            });
            services.AddMassTransitHostedService();
        }
    }
 
}
