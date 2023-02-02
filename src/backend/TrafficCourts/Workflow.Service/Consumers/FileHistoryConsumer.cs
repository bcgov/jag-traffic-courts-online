using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SendEmail message.
    /// </summary>
    public class FileHistoryConsumer : IConsumer<SaveFileHistoryRecord>
    {
        private readonly IFileHistoryService _fileHistoryService;
        private readonly ILogger<FileHistoryConsumer> _logger;

        public FileHistoryConsumer(IFileHistoryService fileHistoryService, ILogger<FileHistoryConsumer> logger)
        {
            _fileHistoryService = fileHistoryService ?? throw new ArgumentNullException(nameof(fileHistoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<SaveFileHistoryRecord> context)
        {
            using var scope = _logger.BeginConsumeScope(context);

            await _fileHistoryService.SaveFileHistoryAsync(context.Message, context.CancellationToken);
        }
    }
}
