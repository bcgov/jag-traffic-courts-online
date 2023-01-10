using MassTransit;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using DisputantUpdateRequest = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputantUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputantUpdateRequestRejectedConsumer : IConsumer<DisputantUpdateRequestRejected>
{
    private readonly ILogger<DisputantUpdateRequestRejectedConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputantUpdateRequestRejectedTemplate _updateRequestRejectedTemplate;

    public DisputantUpdateRequestRejectedConsumer(
        ILogger<DisputantUpdateRequestRejectedConsumer> logger,
        IOracleDataApiService oracleDataApiService,
        IDisputantUpdateRequestRejectedTemplate updateRequestRejectedTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestRejectedTemplate = updateRequestRejectedTemplate ?? throw new ArgumentNullException(nameof(updateRequestRejectedTemplate));
    }

    public async Task Consume(ConsumeContext<DisputantUpdateRequestRejected> context)
    {
        // TCVP-1974
        // - call oracle-data-api to update DisputantUpdateRequest status.
        // - send confirmation email indicating request was rejected
        // - populate file/email history records

        _logger.LogDebug("Consuming message");
        DisputantUpdateRequestRejected message = context.Message;

        // Set the status of the DisputantUpdateRequest object to REJECTED.
        DisputantUpdateRequest updateRequest = await _oracleDataApiService.UpdateDisputantUpdateRequestStatusAsync(message.UpdateRequestId, DisputantUpdateRequestStatus.REJECTED, context.CancellationToken);

        // Get the current Dispute by id
        Dispute dispute = await _oracleDataApiService.GetDisputeByIdAsync(updateRequest.DisputeId, context.CancellationToken);

        // send confirmation email to end user indicating their request was rejected
        if (dispute.EmailAddressVerified)
        {
            PublishEmailConfirmation(dispute, context);
        }

        // populate file history
        PublishFileHistoryLog(dispute, context);
    }

    private async void PublishEmailConfirmation(Dispute dispute, ConsumeContext<DisputantUpdateRequestRejected> context)
    {
        SendDispuantEmail message = new()
        {
            Message = _updateRequestRejectedTemplate.Create(dispute),
            NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
            TicketNumber = dispute.TicketNumber
        };
        await context.PublishWithLog(_logger, message, context.CancellationToken);
    }

    private async void PublishFileHistoryLog(Dispute dispute, ConsumeContext<DisputantUpdateRequestRejected> context)
    {
        SaveFileHistoryRecord fileHistoryRecord = new()
        {
            TicketNumber = dispute.TicketNumber,
            Description = "Disputant update request rejected."
        };
        await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);
    }
}
