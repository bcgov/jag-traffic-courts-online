using AutoMapper;
using MassTransit;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    /// Adds citizen-submitted disputes to the database.
    /// </summary>
    public class SubmitNoticeOfDisputeConsumer : IConsumer<SubmitNoticeOfDispute>
    {
        private readonly ILogger<SubmitNoticeOfDisputeConsumer> _logger;
        private readonly IOracleDataApiService _oracleDataApiService;
        private readonly IMapper _mapper;

        public SubmitNoticeOfDisputeConsumer(ILogger<SubmitNoticeOfDisputeConsumer> logger, IOracleDataApiService oracleDataApiService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Consume(ConsumeContext<SubmitNoticeOfDispute> context)
        {
            using var loggingScope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeGuid);

            try
            {
                _logger.LogDebug("Consuming message");

                Dispute dispute = _mapper.Map<Dispute>(context.Message);

                _logger.LogTrace("TRY CREATING DISPUTE: {@Dispute}", dispute);

                var disputeId = await _oracleDataApiService.SaveDisputeAsync(dispute, context.CancellationToken);
                if (disputeId > 0)
                {
                    _logger.LogDebug("Dispute has been saved with {DisputeId}: ", disputeId);

                    // save file history
                    await context.PublishWithLog(_logger, new SaveFileHistoryRecord
                    {
                        DisputeId = disputeId,
                        AuditLogEntryType = FileHistoryAuditLogEntryType.SUB, // Dispute submitted for staff review
                        ActionByApplicationUser = "Disputant"
                    }, context.CancellationToken);
                    
                    // send notification that a dispute has been created (which will kick off email validation)
                    await context.PublishWithLog(_logger, new DisputeCreated
                    {
                        NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid,
                        TicketNumber = context.Message.TicketNumber,
                        EmailAddress = context.Message.EmailAddress
                    }, context.CancellationToken);
                }
                else
                {
                    _logger.LogDebug("Failed to save the dispute");

                    // TODO handle this better
                    // SubmitNoticeOfDisputeFailed
                    await context.Publish<DisputeRejected>(new
                    {
                        Reason = "Bad request"
                    }, context.CancellationToken);
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
