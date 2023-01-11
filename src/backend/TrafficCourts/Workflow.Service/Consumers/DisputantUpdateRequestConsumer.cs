using MassTransit;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using DisputantUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputantUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputantUpdateRequestConsumer : IConsumer<DisputantUpdateRequest>
{
    private readonly ILogger<DisputantUpdateRequestConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputantUpdateRequestReceivedTemplate _updateRequestReceivedTemplate;

    public DisputantUpdateRequestConsumer(
        ILogger<DisputantUpdateRequestConsumer> logger,
        IOracleDataApiService oracleDataApiService,
        IDisputantUpdateRequestReceivedTemplate updateRequestReceivedTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestReceivedTemplate = updateRequestReceivedTemplate ?? throw new ArgumentNullException(nameof(updateRequestReceivedTemplate));
    }

    public async Task Consume(ConsumeContext<DisputantUpdateRequest> context)
    {
        _logger.LogDebug("Consuming message");
        DisputantUpdateRequest message = context.Message;

        Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(message.NoticeOfDisputeGuid, context.CancellationToken);
        if (dispute is null)
        {
            _logger.LogError($"Dispute was not found for {message.NoticeOfDisputeGuid}");
            return;
        }

        Common.OpenAPIs.OracleDataApi.v1_0.DisputantUpdateRequest disputantUpdateRequest = new()
        {
            UpdateType = DisputantUpdateRequestUpdateType.UNKNOWN,
            Status = DisputantUpdateRequestStatus2.PENDING,
            UpdateJson = JsonSerializer.Serialize(message)
        };

        if (message.EmailAddress is not null)
        {
            // If there was a change of emailAddress, either a change of text or to/from blank ...
            if (message.EmailAddress != dispute?.EmailAddress)
            {
                if (string.IsNullOrWhiteSpace(message.EmailAddress))
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dispute.EmailAddress = null;
                    dispute.EmailAddressVerified = true;
                    await _oracleDataApiService.UpdateDisputeAsync(dispute.DisputeId, dispute, context.CancellationToken);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
                else
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    // Set the emailAddress and reset the verified flag to false in the database
                    dispute = await _oracleDataApiService.ResetDisputeEmailAsync(dispute.DisputeId, message.EmailAddress, context.CancellationToken);

                    // TCVP-2009: Start email saga to update email address
                    await context.PublishWithLog(_logger, new RequestEmailVerification()
                    {
                        EmailAddress = message.EmailAddress,
                        IsUpdateEmailVerification = true,
                        NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
                        TicketNumber = dispute.TicketNumber,
                        DisputeId = dispute.DisputeId
                    }, context.CancellationToken);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
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
                SendDispuantEmail emailMessage = new()
                {
                    Message = _updateRequestReceivedTemplate.Create(dispute),
                    NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
                    TicketNumber = dispute.TicketNumber
                };
                await context.PublishWithLog(_logger, emailMessage, context.CancellationToken);
            }
        }
    }
}
