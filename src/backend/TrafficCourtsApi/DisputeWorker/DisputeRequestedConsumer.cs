using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TrafficCourts.Common.Contract;
using System.Text.Json;
using System.Threading;

namespace DisputeWorker
{
    public class DisputeRequestedConsumer : IConsumer<Dispute>
    {
        private readonly ILogger<DisputeRequestedConsumer> _logger;
        public DisputeRequestedConsumer(ILogger<DisputeRequestedConsumer> logger)
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
