using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TrafficCourts.Common.Contract;
using System.Text.Json;
using System.Threading;

namespace DisputeWorker
{
    public class DisputeOrderedConsumer : IConsumer<Dispute>
    {
        private readonly ILogger<DisputeOrderedConsumer> _logger;
        public DisputeOrderedConsumer(ILogger<DisputeOrderedConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<Dispute> context)
        {
            try
            {
                Dispute dispute = context.Message;
                _logger.LogInformation("receive {dispute}", JsonSerializer.Serialize<Dispute>(dispute));
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductChangedConsumerError", ex);
            }

        }
    }



}
