using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;
using TrafficCourts.Workflow.Service.Services;
using ViolationTicketCount = TrafficCourts.Workflow.Service.Models.ViolationTicketCount;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SubmitDispute message.
    /// </summary>
    public class SubmitDisputeConsumer : IConsumer<SubmitDispute>
    {
        private readonly ILogger<SubmitDisputeConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;

        public SubmitDisputeConsumer(ILogger<SubmitDisputeConsumer> logger, IOracleDataApiService oracleDataApiService)
        {
            _logger = logger;
            _oracleDataApiService = oracleDataApiService;
        }

        public async Task Consume(ConsumeContext<SubmitDispute> context)
        {
            if (context.RequestId != null)
            {
                _logger.LogDebug("Consuming message: {MessageId}", context.MessageId);

                //_logger.LogDebug("TRY CREATING DISPUTE: {Dispute}", dispute.ToString());

                var disputeId = await _oracleDataApiService.CreateDisputeAsync(context.Message);

                if (disputeId != Guid.Empty)
                {
                    _logger.LogDebug("Dispute has been saved with {DisputeId}: ", disputeId);

                    await context.RespondAsync<DisputeSubmitted>(new
                    {
                        context.MessageId,
                        InVar.Timestamp,
                        DisputeId = disputeId
                    });
                }
                else
                {
                    _logger.LogDebug("Failed to save the dispute");

                    await context.RespondAsync<DisputeRejected>(new
                    {
                        context.MessageId,
                        InVar.Timestamp,
                        Reason = "Bad request"
                    });
                }
                
            }
        }
    }
}
