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
    public class DisputeSubmitConsumer : IConsumer<SubmitNoticeOfDispute>
    {
        private readonly ILogger<DisputeSubmitConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly IMapper _mapper;

        public DisputeSubmitConsumer(ILogger<DisputeSubmitConsumer> logger, IOracleDataApiService oracleDataApiService, IMapper mapper)
        {
            _logger = logger;
            _oracleDataApiService = oracleDataApiService;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
        {
            using var messageIdScope = _logger.BeginScope(new Dictionary<string, object> { 
                { "MessageId", context.MessageId! }, 
                { "MessageType", nameof(SubmitNoticeOfDispute) } 
            });

            try
            {
                _logger.LogDebug("Consuming message");

                NoticeOfDispute noticeOfDispute = _mapper.Map<NoticeOfDispute>(context.Message);

                _logger.LogDebug("TRY CREATING DISPUTE: {Dispute}", noticeOfDispute);

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
                _logger.LogError(ex, "Failed to process message");
                throw;
            }
        }
    }
}
