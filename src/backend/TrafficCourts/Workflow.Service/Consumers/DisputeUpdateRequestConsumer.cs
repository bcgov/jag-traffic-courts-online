using MassTransit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection.Metadata;
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
            UpdateJson = JsonConvert.SerializeObject(message, new StringEnumConverter())
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
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUEM, context);
            }
        }

        // If some or all name fields have different data, send a CONTACT_NAME update request
        if (message.ContactGiven1Nm != dispute?.ContactGiven1Nm
            || message.ContactGiven2Nm != dispute?.ContactGiven2Nm
            || message.ContactGiven3Nm!= dispute?.ContactGiven3Nm
            || message.ContactSurnameNm != dispute?.ContactSurnameNm
            || message.ContactLawFirmName != dispute?.ContactLawFirmNm
            || message.DisputantGivenName1 != dispute?.DisputantGivenName1
            || message.DisputantGivenName2 != dispute?.DisputantGivenName2
            || message.DisputantGivenName3 != dispute?.DisputantGivenName3
            || message.DisputantSurname != dispute?.DisputantSurname
            )
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_NAME;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CCON, context);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUPD, context);
        }

        // If some or all address fields have data, send a DISPUTANT_ADDRESS update request
        if (message.AddressLine1 != dispute?.AddressLine1
            || message.AddressLine2 != dispute?.AddressLine2
            || message.AddressLine3 != dispute?.AddressLine3
            || message.AddressCity != dispute?.AddressCity
            || (message.AddressProvince != dispute?.AddressProvince && message.AddressProvinceSeqNo == null)
            || message.PostalCode != dispute?.PostalCode
            || message.AddressProvinceCountryId != dispute?.AddressProvinceCountryId
            || message.AddressProvinceSeqNo != dispute?.AddressProvinceSeqNo
            || message.AddressCountryId != dispute?.AddressCountryId
            || (message.DriversLicenceNumber != dispute?.DriversLicenceNumber && message.DriversLicenceNumber is not null)
            || (message.DriversLicenceProvince != dispute?.DriversLicenceProvince && message.DriversLicenceIssuedProvinceSeqNo is null && message.DriversLicenceNumber is not null)
            || (message.DriversLicenceIssuedCountryId != dispute?.DriversLicenceIssuedCountryId && message.DriversLicenceNumber is not null)
            || (message.DriversLicenceIssuedProvinceSeqNo != dispute?.DriversLicenceIssuedProvinceSeqNo && message.DriversLicenceNumber is not null)
            )
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CCON, context);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUPD, context);
        }

        // If some or all phone fields have data, send a DISPUTANT_PHONE update request
        if (message.HomePhoneNumber != dispute?.HomePhoneNumber)
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_PHONE;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CCON, context);
            PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUPD, context);
        }

        // If some or all court options fields have data, send a COURT_OPTIONS update request
        if (message.RepresentedByLawyer != null && (message.InterpreterLanguageCd != dispute?.InterpreterLanguageCd
            || message.RepresentedByLawyer != (dispute?.RepresentedByLawyer == DisputeRepresentedByLawyer.Y)
            || message.RequestCourtAppearance != dispute?.RequestCourtAppearanceYn
            || message.LawFirmName != dispute?.LawFirmName
            || message.LawyerSurname != dispute?.LawyerSurname
            || message.LawyerGivenName1 != dispute?.LawyerGivenName1
            || message.LawyerGivenName2 != dispute?.LawyerGivenName2
            || message.LawyerGivenName3 != dispute?.LawyerGivenName3
            || message.LawyerAddress != dispute?.LawyerAddress
            || message.LawyerPhoneNumber != dispute?.LawyerPhoneNumber
            || message.LawyerEmail != dispute?.LawyerEmail
            || message.InterpreterRequired != (dispute?.InterpreterRequired == DisputeInterpreterRequired.Y)
            || message.WitnessNo != dispute?.WitnessNo
            || message.FineReductionReason != dispute?.FineReductionReason
            || message.TimeToPayReason != dispute?.TimeToPayReason))
        {
            disputeUpdateRequest.UpdateType = DisputeUpdateRequestUpdateType.COURT_OPTIONS;
            await _oracleDataApiService.SaveDisputeUpdateRequestAsync(message.NoticeOfDisputeGuid.ToString(), disputeUpdateRequest, context.CancellationToken);
            if (message.InterpreterLanguageCd != dispute?.InterpreterLanguageCd ||message.InterpreterRequired != (dispute?.InterpreterRequired == DisputeInterpreterRequired.Y))
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CAIN, context);
            }
            if (message.WitnessNo != dispute?.WitnessNo && (dispute?.WitnessNo == null || message.WitnessNo > dispute.WitnessNo) )
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CAWT, context);
            }
            if (message.WitnessNo != null && (dispute?.WitnessNo != message.WitnessNo) )
            {
                PublishFileHistoryLog(dispute,FileHistoryAuditLogEntryType.CUWT, context);
            }
            if (message.RequestCourtAppearance == DisputeRequestCourtAppearanceYn.N && dispute?.RequestCourtAppearanceYn == DisputeRequestCourtAppearanceYn.Y)
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CCWR, context);
            }
            if (!string.IsNullOrEmpty(message.LawFirmName) && string.IsNullOrEmpty(dispute?.LawFirmName))
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CLEG, context);
            }
            if (!string.IsNullOrEmpty(message.InterpreterLanguageCd) && !string.IsNullOrEmpty(dispute?.InterpreterLanguageCd) && message.InterpreterLanguageCd != dispute.InterpreterLanguageCd) 
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUIN, context);
            }
            if (message.LawFirmName != dispute?.LawFirmName 
                || message.LawyerSurname != dispute?.LawyerSurname
                || message.LawyerGivenName1 != dispute?.LawyerGivenName1
                || message.LawyerGivenName2 != dispute?.LawyerGivenName2
                || message.LawyerGivenName3 != dispute?.LawyerGivenName3
                || message.LawyerAddress != dispute?.LawyerAddress
                || message.LawyerPhoneNumber != dispute?.LawyerPhoneNumber
                || message.LawyerEmail != dispute?.LawyerEmail)
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CULG, context);
            }
            if (message.FineReductionReason != dispute.FineReductionReason || message.TimeToPayReason != dispute.TimeToPayReason)
            {
                PublishFileHistoryLog(dispute, FileHistoryAuditLogEntryType.CUWR, context);
            }
        }

        // If some or all count fields have data, send a DISPUTE_COUNT request
        if (message.DisputeCounts != null && message.DisputeCounts.Count > 0)
        {
            var anyCountUpdated = false;
            foreach(TrafficCourts.Messaging.MessageContracts.DisputeCount disputeCount in message.DisputeCounts)
            {
                TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount? countFound = dispute?.DisputeCounts.FirstOrDefault(x => x.CountNo == disputeCount.CountNo);
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
