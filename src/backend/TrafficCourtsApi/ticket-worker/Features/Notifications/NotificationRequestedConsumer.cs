using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;

namespace Gov.TicketWorker.Features.Notifications
{
    public class NotificationRequestedConsumer : IConsumer<NotificationContract>
    {
        private readonly ILogger<NotificationRequestedConsumer> _logger;
        public NotificationRequestedConsumer(ILogger<NotificationRequestedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<NotificationContract> context)
        {
            try
            {
                NotificationContract n = context.Message;
                if (string.IsNullOrWhiteSpace(n.ViolationTicketNumber))
                {
                    _logger.LogError("notification ticket number is empty");
                }
                _logger.LogInformation("receive requested notification {n}", JsonSerializer.Serialize(n));
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductChangedConsumerError", ex);
            }

            return Task.CompletedTask;
        }
    }
}
