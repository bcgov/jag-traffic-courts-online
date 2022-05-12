using AutoMapper;
using MassTransit;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;
using TrafficCourts.Workflow.Service.Services;
using ViolationTicketCount = TrafficCourts.Workflow.Service.Models.ViolationTicket;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SubmitDispute message.
    /// </summary>
    public class SubmitDisputeConsumer : IConsumer<SubmitNoticeOfDispute>
    {
        private readonly ILogger<SubmitDisputeConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly IMapper _mapper;

        public SubmitDisputeConsumer(ILogger<SubmitDisputeConsumer> logger, IOracleDataApiService oracleDataApiService, IMapper mapper)
        {
            _logger = logger;
            _oracleDataApiService = oracleDataApiService;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
        {
            try
            {
                if (context.MessageId != null)
                {
                    _logger.LogDebug("Consuming message: {MessageId}", context.MessageId);
                }

                NoticeOfDispute noticeOfDispute = _mapper.Map<NoticeOfDispute>(context.Message);

                _logger.LogDebug("TRY CREATING DISPUTE: {Dispute}", noticeOfDispute.ToString());

                var disputeId = await _oracleDataApiService.CreateDisputeAsync(noticeOfDispute);

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
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
            }
        }
    }
}
