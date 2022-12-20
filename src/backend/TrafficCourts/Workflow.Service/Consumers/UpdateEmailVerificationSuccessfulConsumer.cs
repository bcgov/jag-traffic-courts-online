using MassTransit;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using ApiException = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ApiException;

namespace TrafficCourts.Workflow.Service.Consumers;

/// <summary>
/// Consumer for UpdateEmailVerificationSuccessful message (produced when a Disputant confirms their updated email address).
/// This Consumer simply updates the status of DisputantUpdateRequest record(s) for the given NoticeOfDisputeGuid, setting their status from HOLD to PENDING.
/// This Consumer also sends out an email to confirm Disputant's update request(s) are received.
/// </summary>
public class SetDisputantUpdateRequestStatusInDatabase : IConsumer<UpdateEmailVerificationSuccessful>
{
    private readonly ILogger<SetEmailVerifiedOnDisputeInDatabase> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputantUpdateRequestReceivedTemplate _updateRequestReceivedTemplate;

    public SetDisputantUpdateRequestStatusInDatabase(ILogger<SetEmailVerifiedOnDisputeInDatabase> logger, IOracleDataApiService oracleDataApiService, IDisputantUpdateRequestReceivedTemplate updateRequestReceivedTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestReceivedTemplate = updateRequestReceivedTemplate ?? throw new ArgumentNullException(nameof(updateRequestReceivedTemplate));
    }

    public async Task Consume(ConsumeContext<UpdateEmailVerificationSuccessful> context)
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

            // TODO: Enable the service call below once the method that updates status of DisputantUpdateRequest from HOLD to PENDING
            // will be updated to process the operation based on NoticeOfDisputeGuid
            // await _oracleDataApiService.UpdateDisputantUpdateRequestsStatusToPendingAsync(message.NoticeOfDisputeGuid, context.CancellationToken);

            // File History 
            SaveFileHistoryRecord fileHistoryRecord = new SaveFileHistoryRecord();
            fileHistoryRecord.TicketNumber = dispute.TicketNumber;
            fileHistoryRecord.Description = "Update email verification complete";
            await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);

            // File History 
            fileHistoryRecord.Description = "Update request submitted for staff review";
            await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);

            // Send email to disputant to confirm disputant's update request(s) are received and will be reviewed
            EmailMessage emailMessage = _updateRequestReceivedTemplate.Create(dispute);

            await context.PublishWithLog(_logger, new SendDispuantEmail
            {
                Message = emailMessage,
                TicketNumber = dispute.TicketNumber,
                NoticeOfDisputeGuid = message.NoticeOfDisputeGuid
            }, context.CancellationToken);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to update the status of DisputantUpdateRequest(s) to PENDING.");
            throw;
        }
    }
}
