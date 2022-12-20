

using MassTransit;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.Features.Mail;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using DisputantUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputantUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputantUpdateRequestConsumer : IConsumer<DisputantUpdateRequest>
{
    private readonly ILogger<DisputantUpdateRequestConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private static readonly string _pendingDisputantUpdateRequestEmailTemplateName = "DisputantUpdateRequestPendingTemplate";

    public DisputantUpdateRequestConsumer(ILogger<DisputantUpdateRequestConsumer> logger, IOracleDataApiService oracleDataApiService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
    }

    public async Task Consume(ConsumeContext<DisputantUpdateRequest> context)
    {
        _logger.LogDebug("Consuming message");
        DisputantUpdateRequest message = context.Message;

        Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(message.NoticeOfDisputeGuid, context.CancellationToken);
        if (dispute is not null)
        {
            Common.OpenAPIs.OracleDataApi.v1_0.DisputantUpdateRequest disputantUpdateRequest = new()
            {
                UpdateType = DisputantUpdateRequestUpdateType.UNKNOWN,
                Status = DisputantUpdateRequestStatus2.PENDING,
                UpdateJson = JsonSerializer.Serialize(message)
            };

            // If there was a change of emailAddress, either a change of text or to/from blank ...
            if (message.EmailAddress != dispute?.EmailAddress)
            {
                if (message.EmailAddress is null)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dispute.EmailAddress = null;
                    dispute.EmailAddressVerified = true;
                    await _oracleDataApiService.UpdateDisputeAsync(dispute.DisputeId, dispute, context.CancellationToken);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
                else
                {
                    // TODO: Start email saga. TCVP-2009
                }
            }

            // If some or all name fields have data, send a DISPUTANT_NAME update request
            if (!string.IsNullOrEmpty(message.DisputantGivenName1)
                || !string.IsNullOrEmpty(message.DisputantGivenName2)
                || !string.IsNullOrEmpty(message.DisputantGivenName3)
                || !string.IsNullOrEmpty(message.DisputantSurname)
                )
            {
                disputantUpdateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_NAME;
                await _oracleDataApiService.SaveDisputantUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputantUpdateRequest, context.CancellationToken);
            }

            // If some or all address fields have data, send a DISPUTANT_ADDRESS update request
            if (!string.IsNullOrEmpty(message.AddressLine1)
                || !string.IsNullOrEmpty(message.AddressLine2)
                || !string.IsNullOrEmpty(message.AddressLine3)
                || !string.IsNullOrEmpty(message.AddressCity)
                || !string.IsNullOrEmpty(message.AddressProvince)
                || !string.IsNullOrEmpty(message.PostalCode)
                || message.AddressProvinceCountryId is not null
                || message.AddressProvinceSeqNo is not null
                || message.AddressCountryId is not null
                )
            {
                disputantUpdateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_ADDRESS;
                await _oracleDataApiService.SaveDisputantUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputantUpdateRequest, context.CancellationToken);
            }

            // If some or all phone fields have data, send a DISPUTANT_PHONE update request
            if (message.HomePhoneNumber is not null)
            {
                disputantUpdateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_PHONE;
                await _oracleDataApiService.SaveDisputantUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputantUpdateRequest, context.CancellationToken);
            }

            // If at least one disputantUpdateRequest was saved ...
            if (disputantUpdateRequest.UpdateType != DisputantUpdateRequestUpdateType.UNKNOWN)
            {
                if (dispute?.EmailAddressVerified == true && dispute?.EmailAddress is not null)
                {
                    // Send notification email to user that their change request has been submitted
                    PublishEmailConfirmation(dispute, context, _pendingDisputantUpdateRequestEmailTemplateName);
                }
            }
        }
    }

    private async void PublishEmailConfirmation(Dispute? dispute, ConsumeContext<DisputantUpdateRequest> context, string emailTemplate)
    {
        var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == emailTemplate);
        if (template == null)
        {
            _logger.LogError("Email {Template} not found", emailTemplate);
            return;
        }

        if (dispute?.EmailAddress is null)
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
}
