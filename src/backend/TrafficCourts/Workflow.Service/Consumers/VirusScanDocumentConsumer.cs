using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    /// Consumer for VirusScanDocument message.
    /// </summary>
    public class VirusScanDocumentConsumer : IConsumer<VirusScanDocument>
    {
        private readonly IVirusScanService _virusScanService;
        private readonly ILogger<VirusScanDocumentConsumer> _logger;

        public VirusScanDocumentConsumer(IVirusScanService virusScanService, ILogger<VirusScanDocumentConsumer> logger)
        {
            _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<VirusScanDocument> context)
        {
            _logger.LogDebug("Consuming VirusScanDocument message");
            VirusScanDocument message = context.Message;

            using var scope = _logger.BeginConsumeScope(context);
            throw new NotImplementedException();
        }
    }
}
