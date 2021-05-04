using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TrafficCourts.Common.Contract;
using System.Text.Json;

namespace DisputeWorker
{
    public class DisputeUpdatedConsumer : IConsumer<DisputeContract>
    {
        private readonly ILogger<DisputeUpdatedConsumer> _logger;
        public DisputeUpdatedConsumer(ILogger<DisputeUpdatedConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<DisputeContract> context)
        {
            try
            {
                DisputeContract dispute = context.Message;
                _logger.LogInformation("receive updated dispute {dispute}", JsonSerializer.Serialize<DisputeContract>(dispute));
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductChangedConsumerError", ex);
            }

        }
    }



}
