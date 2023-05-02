using MassTransit;
using MassTransit.SagaStateMachine;
using Newtonsoft.Json;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services;
using DisputeUpdateRequest = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputeUpdateRequestAcceptedConsumer : IConsumer<DisputeUpdateRequestAccepted>
{
    private readonly ILogger<DisputeUpdateRequestAcceptedConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputeUpdateRequestAcceptedTemplate _updateRequestAcceptedTemplate;

    public DisputeUpdateRequestAcceptedConsumer(
        ILogger<DisputeUpdateRequestAcceptedConsumer> logger,
        IOracleDataApiService oracleDataApiService,
        IDisputeUpdateRequestAcceptedTemplate updateRequestAcceptedTemplate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestAcceptedTemplate = updateRequestAcceptedTemplate ?? throw new ArgumentNullException(nameof(updateRequestAcceptedTemplate));
    }

    public async Task Consume(ConsumeContext<DisputeUpdateRequestAccepted> context)
    {
        // TCVP-1975
        // - call oracle-data-api to patch the Dispute with the DisputeUpdateRequest changes.
        // - call oracle-data-api to update DisputeUpdateRequest status.
        // - send confirmation email indicating request was accepted
        // - populate file/email history records

        try { 
            using var loggingScope = _logger.BeginConsumeScope(context);

            _logger.LogDebug("Consuming message");
            DisputeUpdateRequestAccepted message = context.Message;

            // Set the status of the DisputeUpdateRequest object to ACCEPTED.
            DisputeUpdateRequest updateRequest = await _oracleDataApiService.UpdateDisputeUpdateRequestStatusAsync(message.UpdateRequestId, DisputeUpdateRequestStatus.ACCEPTED, context.CancellationToken);

            if (updateRequest.UpdateJson is not null)
            {
                // Get the current Dispute by id
                Dispute dispute = await _oracleDataApiService.GetDisputeByIdAsync(updateRequest.DisputeId, false, context.CancellationToken);

                // Extract patched Dispute values per updateType
                UpdateRequest? patch = JsonConvert.DeserializeObject<UpdateRequest>(updateRequest.UpdateJson);
                if (patch == null)
                {
                   throw new InvalidOperationException("Unable to process update request JSON.");
                }

                switch (updateRequest.UpdateType)
                {
                    case DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS:
                        dispute.AddressLine1 = patch.AddressLine1;
                        dispute.AddressLine2 = patch.AddressLine2;
                        dispute.AddressLine3 = patch.AddressLine3;
                        dispute.AddressCity = patch.AddressCity;
                        dispute.AddressProvince = patch.AddressProvince;
                        dispute.PostalCode = patch.PostalCode;
                        dispute.AddressCountryId = patch.AddressCountryId;
                        dispute.AddressProvinceSeqNo = patch.AddressProvinceSeqNo;
                        dispute.AddressProvinceCountryId = patch.AddressProvinceCountryId;
                        if (patch.DriversLicenceNumber is not null) dispute.DriversLicenceNumber = patch.DriversLicenceNumber;
                        if (patch.DriversLicenceProvince is not null) dispute.DriversLicenceProvince = patch.DriversLicenceProvince;
                        break;
                    case DisputeUpdateRequestUpdateType.DISPUTANT_PHONE:
                        dispute.HomePhoneNumber = patch.HomePhoneNumber;
                        break;
                    case DisputeUpdateRequestUpdateType.DISPUTANT_NAME:
                        dispute.DisputantGivenName1 = patch.DisputantGivenName1;
                        dispute.DisputantGivenName2 = patch.DisputantGivenName2;
                        dispute.DisputantGivenName3 = patch.DisputantGivenName3;
                        dispute.DisputantSurname = patch.DisputantSurname;
                        dispute.ContactGiven1Nm = patch.ContactGiven1Nm;
                        dispute.ContactGiven2Nm = patch.ContactGiven2Nm;
                        dispute.ContactGiven3Nm = patch.ContactGiven3Nm;
                        dispute.ContactSurnameNm = patch.ContactSurnameNm;
                        dispute.ContactLawFirmNm = patch.ContactLawFirmNm;
                        dispute.ContactTypeCd = (DisputeContactTypeCd)(patch.ContactType is null ? DisputeContactTypeCd.UNKNOWN : patch.ContactType);
                        break;
                    case DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT:
                        // TODO: update document metadata set StaffReviewStatus to Accepted
                        break;
                    case DisputeUpdateRequestUpdateType.COUNT:
                        foreach (Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount disputeCount in dispute.DisputeCounts)
                        {
                            UpdateDisputeCountRequest? patchCount = patch?.PatchDisputeCounts?.FirstOrDefault(x => x.CountNo == disputeCount.CountNo);
                            if (patchCount != null)
                            {
                                disputeCount.RequestReduction = patchCount.RequestReduction;
                                disputeCount.RequestTimeToPay = patchCount.RequestTimeToPay;
                                disputeCount.PleaCode = patchCount.PleaCode;
                            }
                        }
                        break;
                    case DisputeUpdateRequestUpdateType.COURT_OPTIONS:
                        dispute.RepresentedByLawyer = patch?.RepresentedByLawyer;
                        dispute.LawFirmName = patch?.LawFirmName;
                        dispute.LawyerSurname = patch?.LawyerSurname;
                        dispute.LawyerGivenName1 = patch?.LawyerGivenName1;
                        dispute.LawyerGivenName2 = patch?.LawyerGivenName2;
                        dispute.LawyerGivenName3 = patch?.LawyerGivenName3;
                        dispute.LawyerAddress = patch?.LawyerAddress;
                        dispute.LawyerPhoneNumber = patch?.LawyerPhoneNumber;
                        dispute.LawyerEmail = patch?.LawyerEmail;
                        dispute.RequestCourtAppearanceYn= patch?.RequestCourtAppearanceYn;
                        dispute.InterpreterLanguageCd = patch?.InterpreterLanguageCd;
                        dispute.InterpreterRequired = patch?.InterpreterRequired;
                        dispute.WitnessNo = patch?.WitnessNo;
                        dispute.FineReductionReason = patch?.FineReductionReason;
                        dispute.TimeToPayReason = patch?.TimeToPayReason;
                        dispute.RequestCourtAppearanceYn = patch?.RequestCourtAppearanceYn;
                        break;
                    case DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL:
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
        } catch (Exception ex) {
            throw new Exception("Could not process update request. " + ex.Message, ex);
        }

    }

    private async void PublishEmailConfirmation(Dispute dispute, ConsumeContext<DisputeUpdateRequestAccepted> context)
    {
        SendDisputantEmail message = new()
        {
            Message = _updateRequestAcceptedTemplate.Create(dispute),
            NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
            TicketNumber = dispute.TicketNumber
        };
        await context.PublishWithLog(_logger, message, context.CancellationToken);
    }

    private async void PublishFileHistoryLog(Dispute dispute, ConsumeContext<DisputeUpdateRequestAccepted> context)
    {
        SaveFileHistoryRecord fileHistoryRecord = new()
        {
            DisputeId = dispute.DisputeId,
            AuditLogEntryType = FileHistoryAuditLogEntryType.DURA,
            ActionByApplicationUser = context.Message.UserName
        };
        await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);
    }
}

public class UpdateRequest
{
    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; } 

    public string? AddressLine3 { get; set; }

    public string? AddressCity { get; set; }

    public string? AddressProvince { get; set; }

    public int? AddressProvinceCountryId { get; set; }

    public int? AddressProvinceSeqNo { get; set; }

    public int? AddressCountryId { get; set; }

    public string? PostalCode { get; set; }

    public string? DriversLicenceNumber { get; set; }

    public string? DriversLicenceProvince { get; set; }
    public string? HomePhoneNumber { get; set; }

    public string? DisputantSurname { get; set; }

    public string? DisputantGivenName1 { get; set; }

    public string? DisputantGivenName2 { get; set; }

    public string? DisputantGivenName3 { get; set; }

    public DisputeContactTypeCd? ContactType { get; set; }

    public string? ContactLawFirmNm { get; set; }

    public string? ContactGiven1Nm { get; set; }

    public string? ContactGiven2Nm { get; set; }

    public string? ContactGiven3Nm { get; set; }

    public string? ContactSurnameNm { get; set; }

    public DisputeRepresentedByLawyer? RepresentedByLawyer { get; set; }

    public string? LawFirmName { get; set; }

    public string? LawyerSurname { get; set; }

    public string? LawyerGivenName1 { get; set; }

    public string? LawyerGivenName2 { get; set; }

    public string? LawyerGivenName3 { get; set; }

    public string? LawyerAddress { get; set; }

    public string? LawyerPhoneNumber { get; set; }

    public string? LawyerEmail { get; set; }

    public DisputeRequestCourtAppearanceYn? RequestCourtAppearanceYn { get; set; }

    public string? InterpreterLanguageCd { get; set; }

    public DisputeInterpreterRequired? InterpreterRequired { get; set; }

    public int? WitnessNo { get; set; }

    public string? FineReductionReason { get; set; }

    public string? TimeToPayReason { get; set; }

    public System.Collections.Generic.ICollection<UpdateDisputeCountRequest>? PatchDisputeCounts { get; set; }

}

public partial class UpdateDisputeCountRequest
{
    public int CountNo { get; set; }

    public DisputeCountPleaCode PleaCode { get; set; }

    public DisputeCountRequestTimeToPay RequestTimeToPay { get; set; }

    public DisputeCountRequestReduction RequestReduction { get; set; }

}
