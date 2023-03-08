using MassTransit;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using ApiException = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ApiException;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Consumer for UpdateRequestReceived message (produced when a Disputant's update request(s) are successfully received by the Oracle Data API).
/// This Consumer sends out an email to confirm Disputant's update request(s) are received.
/// </summary>
public class SendUpdateRequestReceivedEmailConsumer : IConsumer<UpdateRequestReceived>
{
    private readonly ILogger<SendUpdateRequestReceivedEmailConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputeUpdateRequestReceivedTemplate _updateRequestReceivedTemplate;

    public SendUpdateRequestReceivedEmailConsumer(ILogger<SendUpdateRequestReceivedEmailConsumer> logger, IOracleDataApiService oracleDataApiService, IDisputeUpdateRequestReceivedTemplate updateRequestReceivedTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestReceivedTemplate = updateRequestReceivedTemplate ?? throw new ArgumentNullException(nameof(updateRequestReceivedTemplate));
    }

    public async Task Consume(ConsumeContext<UpdateRequestReceived> context)
    {
        using var loggingScope = _logger.BeginConsumeScope(context, message => message.NoticeOfDisputeGuid);

        var message = context.Message;
        try
        {
            Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(message.NoticeOfDisputeGuid, context.CancellationToken);
            if (dispute is null)
            {
                _logger.LogInformation("Dispute not found");
                return;
            }

            // File History 
            SaveFileHistoryRecord fileHistoryRecord = new()
            {
                DisputeId = dispute.DisputeId,
                AuditLogEntryType = FileHistoryAuditLogEntryType.ESUR,
                ActionByApplicationUser = dispute.UserAssignedTo
            };
            await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);

            // File History 
            fileHistoryRecord.AuditLogEntryType = FileHistoryAuditLogEntryType.URSR;
            await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);

            // Send email to disputant to confirm disputant's update request(s) are received and will be reviewed
            EmailMessage emailMessage = _updateRequestReceivedTemplate.Create(dispute);

            await context.PublishWithLog(_logger, new SendDisputantEmail
            {
                Message = emailMessage,
                TicketNumber = dispute.TicketNumber,
                NoticeOfDisputeGuid = message.NoticeOfDisputeGuid
            }, context.CancellationToken);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to update the status of DisputeUpdateRequest(s) to PENDING.");
            throw;
        }
    }
}
