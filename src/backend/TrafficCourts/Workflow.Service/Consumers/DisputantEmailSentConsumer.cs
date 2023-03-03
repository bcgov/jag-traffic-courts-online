using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Mappers;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SendEmail message.
    /// </summary>
    public class DisputantEmailSentConsumer : IConsumer<DisputantEmailSent>
    {
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly ILogger<DisputantEmailSentConsumer> _logger;

        public DisputantEmailSentConsumer(IOracleDataApiService oracleDataApiService, ILogger<DisputantEmailSentConsumer> logger)
        {
            _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<DisputantEmailSent> context)
        {
            using var scope = _logger.BeginConsumeScope(context);

            EmailHistory emailHistory = Mapper.ToEmailHistory(context.Message);

            await _oracleDataApiService.CreateEmailHistoryAsync(emailHistory, context.CancellationToken);
        }
    }
}
