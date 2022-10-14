using AutoMapper;
using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    /// Creates the dispute in the database.
    /// </summary>
    public class CreateDisputeInDatabaseConsumer : IConsumer<SubmitNoticeOfDispute>
    {
        private readonly ILogger<CreateDisputeInDatabaseConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly IMapper _mapper;

        public CreateDisputeInDatabaseConsumer(ILogger<CreateDisputeInDatabaseConsumer> logger, IOracleDataApiService oracleDataApiService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
        {
            using var loggingScope = _logger.BeginScope(context, message => message.NoticeOfDisputeId);

            try
            {
                _logger.LogDebug("Consuming message");
                var cancellationToken = context.CancellationToken;

                Dispute dispute = _mapper.Map<Dispute>(context.Message);

                _logger.LogTrace("TRY CREATING DISPUTE: {@Dispute}", dispute);

                var disputeId = await _oracleDataApiService.CreateDisputeAsync(dispute, cancellationToken);

                if (disputeId > 0)
                {
                    _logger.LogDebug("Dispute has been saved with {DisputeId}: ", disputeId);

                    await context.Publish(new NoticeOfDisputeSubmitted
                    {
                        DisputeId = disputeId,
                        NoticeOfDisputeId = context.Message.NoticeOfDisputeId,
                    }, cancellationToken);
                }
                else
                {
                    _logger.LogDebug("Failed to save the dispute");

                    // TODO handle this better
                    // SubmitNoticeOfDisputeFailed
                    await context.Publish<DisputeRejected>(new
                    {
                        Reason = "Bad request"
                    }, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message");
                throw;
            }
        }
    }
}
