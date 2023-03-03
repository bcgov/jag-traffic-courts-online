using MassTransit;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using DisputeUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputeUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputeUpdateRequestConsumer : IConsumer<DisputeUpdateRequest>
{
    private readonly ILogger<DisputeUpdateRequestConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputeUpdateRequestReceivedTemplate _updateRequestReceivedTemplate;

    public DisputeUpdateRequestConsumer(
        ILogger<DisputeUpdateRequestConsumer> logger,
        IOracleDataApiService oracleDataApiService,
        IDisputeUpdateRequestReceivedTemplate updateRequestReceivedTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestReceivedTemplate = updateRequestReceivedTemplate ?? throw new ArgumentNullException(nameof(updateRequestReceivedTemplate));
    }

    public async Task Consume(ConsumeContext<DisputeUpdateRequest> context)
    {
        _logger.LogDebug("Consuming message");
        DisputeUpdateRequest message = context.Message;

        Dispute? dispute = await _oracleDataApiService.GetDisputeByNoticeOfDisputeGuidAsync(message.NoticeOfDisputeGuid, context.CancellationToken);
        if (dispute is null)
        {
            _logger.LogError($"Dispute was not found for {message.NoticeOfDisputeGuid}");
            return;
        }

        Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest disputeUpdateRequest = new()
        {
            UpdateType = DisputeUpdateRequestUpdateType.UNKNOWN,
            Status = DisputeUpdateRequestStatus2.PENDING,
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

        // If some or all name fields have data, send a CONTACT_NAME update request
        if (!string.IsNullOrEmpty(message.ContactGiven1Nm)
            || !string.IsNullOrEmpty(message.ContactGiven2Nm)
            || !string.IsNullOrEmpty(message.ContactGiven3Nm)
            || !string.IsNullOrEmpty(message.ContactSurnameNm)
            || !string.IsNullOrEmpty(message.ContactLawFirmName)
            || !string.IsNullOrEmpty(message.DisputantGivenName1)
            || !string.IsNullOrEmpty(message.DisputantGivenName2)
            || !string.IsNullOrEmpty(message.DisputantGivenName3)
            || !string.IsNullOrEmpty(message.DisputantSurname)
            )
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_NAME;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
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
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
        }

        // If some or all phone fields have data, send a DISPUTANT_PHONE update request
        if (message.HomePhoneNumber is not null)
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_PHONE;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
        }

        // If some or all court options fields have data, send a COURT_OPTIONS update request
        if (!string.IsNullOrEmpty(message.InterpreterLanguageCd)
            || message.RepresentedByLawyer != null
            || !string.IsNullOrEmpty(message.LawFirmName)
            || !string.IsNullOrEmpty(message.LawyerSurname)
            || !string.IsNullOrEmpty(message.LawyerGivenName1)
            || !string.IsNullOrEmpty(message.LawyerGivenName2)
            || !string.IsNullOrEmpty(message.LawyerGivenName3)
            || !string.IsNullOrEmpty(message.LawyerAddress)
            || !string.IsNullOrEmpty(message.LawyerPhoneNumber)
            || !string.IsNullOrEmpty(message.LawyerEmail)
            || message.InterpreterRequired != null
            || message.WitnessNo != null
            || !string.IsNullOrEmpty(message.FineReductionReason)
            || !string.IsNullOrEmpty(message.TimeToPayReason))
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.COURT_OPTIONS;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
        }

        // If some or all count fields have data, send a DISPUTE_COUNT request
        if (message.DisputeCounts != null && message.DisputeCounts.Count > 0)
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.COUNT;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
        }

        // If the message contains a documentId, send a DISPUTANT_DOCUMENT request
        if (message.DocumentId is not null && message.DocumentId != Guid.Empty)
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
        }

        // TODO: ensure security so only requests authenticated with BCSC can do COURT_OPTIONS, COUNT, DOCUMENTS

        // If at least one DisputeUpdateRequest was saved ...
        if (disputeUpdateRequest.UpdateType != DisputeUpdateRequestUpdateType.UNKNOWN)
        {
            if (dispute?.EmailAddressVerified == true && dispute?.EmailAddress is not null)
            {
                // Send notification email to user that their change request has been submitted
                SendDisputantEmail emailMessage = new()
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
