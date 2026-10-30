using AutoMapper;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Services.EmailTemplates;
using DisputeUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputeUpdateRequest;

namespace TrafficCourts.Workflow.Service.Consumers;

public class DisputeUpdateRequestConsumer : IConsumer<DisputeUpdateRequest>
{
    private readonly ILogger<DisputeUpdateRequestConsumer> _logger;
    private readonly IOracleDataApiService _oracleDataApiService;
    private readonly IDisputeUpdateRequestReceivedTemplate _updateRequestReceivedTemplate;
    private readonly IMapper _mapper;

    public DisputeUpdateRequestConsumer(
        ILogger<DisputeUpdateRequestConsumer> logger,
        IOracleDataApiService oracleDataApiService,
        IDisputeUpdateRequestReceivedTemplate updateRequestReceivedTemplate,
        IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _oracleDataApiService = oracleDataApiService ?? throw new ArgumentNullException(nameof(oracleDataApiService));
        _updateRequestReceivedTemplate = updateRequestReceivedTemplate ?? throw new ArgumentNullException(nameof(updateRequestReceivedTemplate));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        // TCVP-2497 Map current state of the dispute fields to DisputeUpdateRequest type in order to save as CurrentJson for comparison with UpdateJson.
        DisputeUpdateRequest currentDispute = _mapper.Map<DisputeUpdateRequest>(dispute);

        // Fields that are not part of the citizen update dispute flow must always match the current
        // dispute so they never appear as changed in the staff portal diff view.
        message.RequestCourtAppearance = currentDispute.RequestCourtAppearance;
        message.FineReductionReason = currentDispute.FineReductionReason;
        message.TimeToPayReason = currentDispute.TimeToPayReason;

        // For each additional sub-section, if the citizen selected "No change" (field is null),
        // copy the current dispute's values so they are identical in UpdateJson vs CurrentJson
        if (message.RepresentedByLawyer is null)
        {
            message.RepresentedByLawyer = currentDispute.RepresentedByLawyer;
            message.LawFirmName = currentDispute.LawFirmName;
            message.LawyerSurname = currentDispute.LawyerSurname;
            message.LawyerGivenName1 = currentDispute.LawyerGivenName1;
            message.LawyerGivenName2 = currentDispute.LawyerGivenName2;
            message.LawyerGivenName3 = currentDispute.LawyerGivenName3;
            message.LawyerAddress = currentDispute.LawyerAddress;
            message.LawyerPhoneNumber = currentDispute.LawyerPhoneNumber;
            message.LawyerEmail = currentDispute.LawyerEmail;
        }

        if (message.InterpreterRequired is null)
        {
            message.InterpreterRequired = currentDispute.InterpreterRequired;
            message.InterpreterLanguageCd = currentDispute.InterpreterLanguageCd;
        }

        if (message.WitnessNo is null)
        {
            message.WitnessNo = currentDispute.WitnessNo;
        }

        TrafficCourts.Domain.Models.DisputeUpdateRequest disputeUpdateRequest = new()
        {
            UpdateType = DisputeUpdateRequestUpdateType.UNKNOWN,
            Status = DisputeUpdateRequestStatus2.PENDING,
            UpdateJson = JsonConvert.SerializeObject(message, new StringEnumConverter()),
            CurrentJson = JsonConvert.SerializeObject(currentDispute, new StringEnumConverter())
        };

        if (message.ContactSectionEnabled && message.EmailAddress is not null)
        {
            // If there was a change of emailAddress, either a change of text or to/from blank ...
            if (message.EmailAddress != dispute.EmailAddress)
            {
                if (string.IsNullOrWhiteSpace(message.EmailAddress))
                {
                    dispute.EmailAddress = null;
                    dispute.EmailAddressVerified = true;
                    await _oracleDataApiService.UpdateDisputeAsync(dispute.DisputeId, dispute, false, context.CancellationToken);
                }
                else
                {
                    // Set the emailAddress and reset the verified flag to false in the database
                    dispute = await _oracleDataApiService.ResetDisputeEmailAsync(dispute.DisputeId, message.EmailAddress, context.CancellationToken);

                    // TCVP-2009: Start email saga to update email address
                    await context.PublishWithLog(_logger, new RequestEmailVerification()
                    {
                        EmailAddress = message.EmailAddress,
                        IsUpdateEmailVerification = true,
                        NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
                        TicketNumber = dispute.TicketNumber
                    }, context.CancellationToken);
                }
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUEM, context);
            }
        }

        // If contact type or contact person fields have changed, send a DISPUTANT_NAME update request.
        // Note: disputant_surname and disputant_given_names are intentionally excluded — citizens cannot
        // update their own name through the update dispute flow.
        if (message.ContactSectionEnabled && (message.ContactType != dispute.ContactTypeCd
            || message.ContactLawFirmName != dispute.ContactLawFirmNm

            || message.ContactGiven1Nm != dispute.ContactGiven1Nm
            || message.ContactGiven2Nm != dispute.ContactGiven2Nm
            || message.ContactGiven3Nm != dispute.ContactGiven3Nm
            || message.ContactSurnameNm != dispute.ContactSurnameNm
            ))
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_NAME;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CCON, context);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUPD, context);
        }

        // If some or all address fields have data, send a DISPUTANT_ADDRESS update request
        if (message.ContactSectionEnabled && !message.AddressLine1.IsNullOrEmpty() && (message.AddressLine1 != dispute.AddressLine1
            || message.AddressLine2 != dispute.AddressLine2
            || message.AddressLine3 != dispute.AddressLine3
            || message.AddressCity != dispute.AddressCity
            || (message.AddressProvince != dispute.AddressProvince && message.AddressProvinceSeqNo == null)
            || message.PostalCode != dispute.PostalCode
            || message.AddressProvinceCountryId != dispute.AddressProvinceCountryId
            || message.AddressProvinceSeqNo != dispute.AddressProvinceSeqNo
            || message.AddressCountryId != dispute.AddressCountryId
            || message.DriversLicenceNumber != dispute.DriversLicenceNumber
            || (message.DriversLicenceProvince != dispute.DriversLicenceProvince && message.DriversLicenceIssuedProvinceSeqNo is null)
            || message.DriversLicenceIssuedCountryId != dispute.DriversLicenceIssuedCountryId
            || message.DriversLicenceIssuedProvinceSeqNo != dispute.DriversLicenceIssuedProvinceSeqNo
            ))
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CCON, context);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUPD, context);
        }

        // If some or all phone fields have data, send a DISPUTANT_PHONE update request
        if (message.ContactSectionEnabled && message.HomePhoneNumber != dispute.HomePhoneNumber && !message.HomePhoneNumber.IsNullOrEmpty()) 
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_PHONE;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CCON, context);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUPD, context);
        }

        // If some or all court options fields have data, send a COURT_OPTIONS update request.
        // Each sub-section is gated independently: a null value means the citizen selected "No change"
        // and that sub-section should not be processed.
        bool lawyerChanged = message.AdditionalSectionEnabled && message.RepresentedByLawyer != null && (
            message.RepresentedByLawyer != (dispute.RepresentedByLawyer == DisputeRepresentedByLawyer.Y)
            || message.LawFirmName != dispute.LawFirmName
            || message.LawyerSurname != dispute.LawyerSurname
            || message.LawyerGivenName1 != dispute.LawyerGivenName1
            || message.LawyerGivenName2 != dispute.LawyerGivenName2
            || message.LawyerGivenName3 != dispute.LawyerGivenName3
            || message.LawyerAddress != dispute.LawyerAddress
            || message.LawyerPhoneNumber != dispute.LawyerPhoneNumber
            || message.LawyerEmail != dispute.LawyerEmail);

        bool interpreterChanged = message.AdditionalSectionEnabled && message.InterpreterRequired != null && (
            message.InterpreterRequired != (dispute.InterpreterRequired == DisputeInterpreterRequired.Y)
            || (message.InterpreterRequired == true && message.InterpreterLanguageCd != dispute.InterpreterLanguageCd));

        bool witnessChanged = message.AdditionalSectionEnabled && message.WitnessNo != null && message.WitnessNo != dispute.WitnessNo;

        if (lawyerChanged || interpreterChanged || witnessChanged)
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.COURT_OPTIONS;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            if (message.InterpreterLanguageCd != dispute.InterpreterLanguageCd ||message.InterpreterRequired != (dispute.InterpreterRequired == DisputeInterpreterRequired.Y))
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CAIN, context);
            }
            if (message.WitnessNo != dispute.WitnessNo && (dispute.WitnessNo == null || message.WitnessNo > dispute.WitnessNo) )
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CAWT, context);
            }
            if (message.WitnessNo != null && (dispute.WitnessNo != message.WitnessNo) )
            {
                PublishFileHistoryLog(dispute,FileHistoryAuditLogEntryType.CUWT, context);
            }
            if (!string.IsNullOrEmpty(message.LawFirmName) && string.IsNullOrEmpty(dispute.LawFirmName))
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CLEG, context);
            }
            if (!string.IsNullOrEmpty(message.InterpreterLanguageCd) && !string.IsNullOrEmpty(dispute.InterpreterLanguageCd) && message.InterpreterLanguageCd != dispute.InterpreterLanguageCd) 
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUIN, context);
            }
            if (message.LawFirmName != dispute.LawFirmName 
                || message.LawyerSurname != dispute.LawyerSurname
                || message.LawyerGivenName1 != dispute.LawyerGivenName1
                || message.LawyerGivenName2 != dispute.LawyerGivenName2
                || message.LawyerGivenName3 != dispute.LawyerGivenName3
                || message.LawyerAddress != dispute.LawyerAddress
                || message.LawyerPhoneNumber != dispute.LawyerPhoneNumber
                || message.LawyerEmail != dispute.LawyerEmail)
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CULG, context);
            }
        }

        // If some or all count fields have data, send a DISPUTE_COUNT request
        // This shouldn't happen any more as of 2.24 - Citizens portal no longer allows COUNT changes via fields - uses VTWR document attachment instead
        if (message.DisputeCounts != null && message.DisputeCounts.Count > 0)
        {
            var anyCountUpdated = false;
            foreach(TrafficCourts.Messaging.MessageContracts.DisputeCount disputeCount in message.DisputeCounts)
            {
                TrafficCourts.Domain.Models.DisputeCount? countFound = dispute?.DisputeCounts.FirstOrDefault(x => x.CountNo == disputeCount.CountNo);
                if (countFound != null)
                {
                    if (disputeCount.PleaCode != countFound.PleaCode || disputeCount.RequestReduction != countFound.RequestReduction || disputeCount.RequestTimeToPay != countFound.RequestTimeToPay) 
                    { 
                        anyCountUpdated = true;
                    }
                }
            }
            if (anyCountUpdated == true)
            {
                disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.COUNT;
                await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            }
        }

        // If the message contains any documentId, send a DISPUTANT_DOCUMENT request for them
        if (message.UploadedDocuments?.Count > 0)
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
        }

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

    private async void PublishFileHistoryLog(Dispute dispute, FileHistoryAuditLogEntryType logType, ConsumeContext<DisputeUpdateRequest> context)
    {
        SaveFileHistoryRecord fileHistoryRecord = new()
        {
            DisputeId = dispute.DisputeId,
            NoticeOfDisputeId = dispute.NoticeOfDisputeGuid,
            AuditLogEntryType = logType,
            ActionByApplicationUser = "Disputant"
        };
        await context.PublishWithLog(_logger, fileHistoryRecord, context.CancellationToken);
    }
}
