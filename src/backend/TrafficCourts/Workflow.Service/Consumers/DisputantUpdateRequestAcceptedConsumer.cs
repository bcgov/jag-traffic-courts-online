using MassTransit;
using Newtonsoft.Json;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using DisputantUpdateRequest = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputantUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputantUpdateRequestAcceptedConsumer : IConsumer<DisputantUpdateRequestAccepted>
{
    private readonly ILogger<DisputantUpdateRequestAcceptedConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputantUpdateRequestAcceptedTemplate _updateRequestAcceptedTemplate;

    public DisputantUpdateRequestAcceptedConsumer(
        ILogger<DisputantUpdateRequestAcceptedConsumer> logger,
        IOracleDataApiService oracleDataApiService,
        IDisputantUpdateRequestAcceptedTemplate updateRequestAcceptedTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestAcceptedTemplate = updateRequestAcceptedTemplate ?? throw new ArgumentNullException(nameof(updateRequestAcceptedTemplate));
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
                    dispute.ContactGiven1Nm = patch?.ContactGiven1Nm;
                    dispute.ContactGiven2Nm = patch?.ContactGiven2Nm;
                    dispute.ContactGiven3Nm = patch?.ContactGiven3Nm;
                    dispute.ContactSurnameNm = patch?.ContactSurnameNm;
                    dispute.ContactLawFirmNm = patch?.ContactLawFirmNm;
                    dispute.ContactTypeCd = (DisputeContactTypeCd)(patch?.ContactTypeCd);
                    break;
                case DisputantUpdateRequestUpdateType.DISPUTANT_DOCUMENT:
                    // TODO: update document metadata set StaffReviewStatus to Accepted
                    break;
                case DisputantUpdateRequestUpdateType.COUNT:
                    foreach(Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount disputeCount in dispute.DisputeCounts)
                    {
                        Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount? patchCount = patch?.DisputeCounts.FirstOrDefault(x => x.CountNo == disputeCount.CountNo);
                        if (patchCount != null)
                        {
                            disputeCount.RequestCourtAppearance = patchCount.RequestCourtAppearance;
                            disputeCount.RequestReduction = patchCount.RequestReduction;
                            disputeCount.RequestTimeToPay = patchCount.RequestTimeToPay;
                        }
                    }
                    break;
                case DisputantUpdateRequestUpdateType.COURT_OPTIONS:
                    dispute.RepresentedByLawyer = patch?.RepresentedByLawyer;
                    dispute.LawFirmName = patch?.LawFirmName;
                    dispute.LawyerSurname = patch?.LawyerSurname;
                    dispute.LawyerGivenName1 = patch?.LawyerGivenName1;
                    dispute.LawyerGivenName2 = patch?.LawyerGivenName2;
                    dispute.LawyerGivenName3 = patch?.LawyerGivenName3;
                    dispute.LawyerAddress = patch?.LawyerAddress;
                    dispute.LawyerPhoneNumber = patch?.LawyerPhoneNumber;
                    dispute.LawyerEmail = patch?.LawyerEmail;
                    dispute.InterpreterLanguageCd = patch?.InterpreterLanguageCd;
                    dispute.InterpreterRequired = patch?.InterpreterRequired;   
                    dispute.WitnessNo = patch?.WitnessNo;
                    dispute.FineReductionReason = patch?.FineReductionReason;
                    dispute.TimeToPayReason = patch?.TimeToPayReason;
#pragma warning disable CS8629 // Nullable value type may be null.
                    dispute.RequestCourtAppearanceYn = (DisputeRequestCourtAppearanceYn)(patch?.RequestCourtAppearanceYn);
#pragma warning restore CS8629 // Nullable value type may be null.
                    break;
                case DisputantUpdateRequestUpdateType.DISPUTANT_EMAIL:
                    // nothing to do here except record in file history (down further)
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
        SendDispuantEmail message = new()
        {
            Message = _updateRequestAcceptedTemplate.Create(dispute),
            NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
            TicketNumber = dispute.TicketNumber
        };
        await context.PublishWithLog(_logger, message, context.CancellationToken);
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
