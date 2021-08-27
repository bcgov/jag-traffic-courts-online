using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TrafficCourts.Common.Contract;
using System.Text.Json;

namespace Gov.TicketWorker.Features.Disputes
{
    public class DisputeUpdatedConsumer : IConsumer<DisputeContract>
    {
        private readonly ILogger<DisputeUpdatedConsumer> _logger;
        public DisputeUpdatedConsumer(ILogger<DisputeUpdatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<DisputeContract> context)
        {
            try
            {
                DisputeContract dispute = context.Message;
                _logger.LogInformation("receive updated dispute {dispute}", JsonSerializer.Serialize(dispute));
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductChangedConsumerError", ex);
            }

            return Task.CompletedTask;
        }
    }
}
