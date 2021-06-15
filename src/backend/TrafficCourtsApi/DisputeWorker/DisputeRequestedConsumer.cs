using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TrafficCourts.Common.Contract;
using System.Text.Json;

namespace DisputeWorker
{
    public class DisputeRequestedConsumer : IConsumer<DisputeContract>
    {
        private readonly ILogger<DisputeRequestedConsumer> _logger;
        public DisputeRequestedConsumer(ILogger<DisputeRequestedConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<DisputeContract> context)
        {
            try
            {
                DisputeContract dispute = context.Message;
                _logger.LogInformation("receive requested dispute {dispute}", JsonSerializer.Serialize(dispute));
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductChangedConsumerError", ex);
            }

            return Task.CompletedTask;
        }
    }
}
