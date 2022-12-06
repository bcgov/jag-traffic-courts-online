using AutoMapper;
using MassTransit;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputantUpdateRequestAcceptedConsumer : IConsumer<DisputantUpdateRequestAccepted>
{
    private readonly ILogger<DisputantUpdateRequestAccepted> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IMapper _mapper;
    private static readonly string _approvedDisputantUpdateRequestEmailTemplateName = "DisputantUpdateRequestApprovedTemplate";

    public DisputantUpdateRequestAcceptedConsumer(ILogger<DisputantUpdateRequestAccepted> logger, IOracleDataApiService oracleDataApiService, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task Consume(ConsumeContext<DisputantUpdateRequestAccepted> context)
    {
        // TCVP-1975
        // - call oracle-data-api to patch the Dispute with the DisputantUpdateRequest changes.
        // - call oracle-data-api to update DisputantUpdateRequest status.
        // - send confirmation email indicating request was accepted
        // - populate file/email history records

        _logger.LogDebug("Consuming message");
        DisputantUpdateRequestAccepted message = context.Message;

        // Set the status of the DisputantUpdateRequest object to ACCEPTED.
        DisputantUpdateRequest updateRequest = await _oracleDataApiService.UpdateDisputantUpdateRequestStatusAsync(message.UpdateRequestId, DisputantUpdateRequestStatus.ACCEPTED, context.CancellationToken);

        // TODO: patch Dispute with changes in the updateRequest object

        // Get the updated Dispute by id (note, this may not be needed if the patch returns the full Dispute object)
        Dispute dispute = await _oracleDataApiService.GetDisputeByIdAsync(updateRequest.DisputeId, context.CancellationToken);

        // send confirmation email to end user indicating their request was accepted
        PublishEmailConfirmation(dispute, context);

        // populate file history
        PublishFileHistoryLog(dispute, context);
    }

    private async void PublishEmailConfirmation(Dispute dispute, ConsumeContext<DisputantUpdateRequestAccepted> context)
    {
        var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == _approvedDisputantUpdateRequestEmailTemplateName);
        if (template == null)
        {
            _logger.LogError("Email {Template} not found", _approvedDisputantUpdateRequestEmailTemplateName);
            return;
        }

        if (dispute.EmailAddress is null)
        {
            _logger.LogError("EmailAddress is null on Dispute");
            return;
        }

        var emailMessage = new EmailMessage()
        {
            From = template.Sender,
            To = dispute.EmailAddress,
            Subject = template.SubjectTemplate,
            TextContent = template.PlainContentTemplate,
            HtmlContent = template.HtmlContentTemplate,
        };

        await context.PublishWithLog(_logger, emailMessage, context.CancellationToken);
    }

    private async void PublishFileHistoryLog(Dispute dispute, ConsumeContext<DisputantUpdateRequestAccepted> context)
    {
        SaveFileHistoryRecord fileHistoryRecord = new()
        {
            TicketNumber = dispute.TicketNumber,
            Description = "Disputant update request accepted."
        };
        await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);
    }
}
