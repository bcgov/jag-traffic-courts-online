using AutoMapper;
using MassTransit;
using Newtonsoft.Json;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputantUpdateRequestAcceptedConsumer : IConsumer<DisputantUpdateRequestAccepted>
{
    private readonly ILogger<DisputantUpdateRequestAcceptedConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private static readonly string _acceptedDisputantUpdateRequestEmailTemplateName = "DisputantUpdateRequestAcceptedTemplate";

    public DisputantUpdateRequestAcceptedConsumer(ILogger<DisputantUpdateRequestAcceptedConsumer> logger, IOracleDataApiService oracleDataApiService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
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

        if (updateRequest.UpdateJson is not null)
        {
            // Get the current Dispute by id
            Dispute dispute = await _oracleDataApiService.GetDisputeByIdAsync(updateRequest.DisputeId, context.CancellationToken);

            // Extract patched Dispute values per updateType
            Dispute? patch = JsonConvert.DeserializeObject<Dispute>(updateRequest.UpdateJson);
            switch (updateRequest.UpdateType)
            {
                case DisputantUpdateRequestUpdateType.DISPUTANT_ADDRESS:
                    dispute.AddressLine1 = patch?.AddressLine1;
                    dispute.AddressLine2 = patch?.AddressLine2;
                    dispute.AddressLine3 = patch?.AddressLine3;
                    dispute.AddressCity = patch?.AddressCity;
                    dispute.AddressProvince = patch?.AddressProvince;
                    dispute.PostalCode = patch?.PostalCode;
                    break;
                case DisputantUpdateRequestUpdateType.DISPUTANT_PHONE:
                    dispute.HomePhoneNumber = patch?.HomePhoneNumber;
                    break;
                case DisputantUpdateRequestUpdateType.DISPUTANT_NAME:
                    dispute.DisputantGivenName1 = patch?.DisputantGivenName1;
                    dispute.DisputantGivenName2 = patch?.DisputantGivenName2;
                    dispute.DisputantGivenName3 = patch?.DisputantGivenName3;
                    dispute.DisputantSurname = patch?.DisputantSurname;
                    break;
                default:
                    break;
            }

            // Save changes
            dispute = await _oracleDataApiService.UpdateDisputeAsync(dispute.DisputeId, dispute, context.CancellationToken);

            // send confirmation email to end user indicating their request was accepted
            if (dispute.EmailAddressVerified)
            {
                PublishEmailConfirmation(dispute, context);
            }

            // populate file history
            PublishFileHistoryLog(dispute, context);
        }
    }

    private async void PublishEmailConfirmation(Dispute dispute, ConsumeContext<DisputantUpdateRequestAccepted> context)
    {
        var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == _acceptedDisputantUpdateRequestEmailTemplateName);
        if (template == null)
        {
            _logger.LogError("Email {Template} not found", _acceptedDisputantUpdateRequestEmailTemplateName);
            return;
        }

        if (dispute.EmailAddress is null)
        {
            _logger.LogError("EmailAddress is null on Dispute");
            return;
        }

        SendDispuantEmail emailMessage = new()
        {
            NoticeOfDisputeGuid = new System.Guid(dispute.NoticeOfDisputeGuid),
            TicketNumber = dispute.TicketNumber,
            Message = new EmailMessage()
            {
                From = template.Sender,
                To = dispute.EmailAddress,
                Subject = template.SubjectTemplate,
                TextContent = template.PlainContentTemplate,
                HtmlContent = template.HtmlContentTemplate,
            }
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
