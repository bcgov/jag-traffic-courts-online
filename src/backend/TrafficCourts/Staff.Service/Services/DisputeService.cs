using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Staff.Service.Models;
using System.Collections.ObjectModel;
using System.Security.Claims;
using TrafficCourts.Common.Models;
using TrafficCourts.Coms.Client;
using TrafficCourts.Common.Errors;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public class DisputeService : IDisputeService
{
    private readonly ILogger<DisputeService> _logger;
    private readonly ICancelledDisputeEmailTemplate _cancelledDisputeEmailTemplate;
    private readonly IRejectedDisputeEmailTemplate _rejectedDisputeEmailTemplate;
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly IBus _bus;
    private readonly IObjectManagementService _objectManagementService;


    public DisputeService(
        IOracleDataApiClient oracleDataApi,
        IBus bus,
        IObjectManagementService objectManagementService,
        ICancelledDisputeEmailTemplate cancelledDisputeEmailTemplate,
        IRejectedDisputeEmailTemplate rejectedDisputeEmailTemplate,
        ILogger<DisputeService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _objectManagementService = objectManagementService ?? throw new ArgumentNullException(nameof(objectManagementService));
        _cancelledDisputeEmailTemplate = cancelledDisputeEmailTemplate ?? throw new ArgumentNullException(nameof(cancelledDisputeEmailTemplate));
        _rejectedDisputeEmailTemplate = rejectedDisputeEmailTemplate ?? throw new ArgumentNullException(nameof(rejectedDisputeEmailTemplate));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<Dispute>> GetAllDisputesAsync(ExcludeStatus? excludeStatus, CancellationToken cancellationToken)
    {
        ICollection<Dispute> disputes = await _oracleDataApi.GetAllDisputesAsync(null, excludeStatus, cancellationToken);

        foreach(Dispute dispute in disputes)
        {
            try
            {
                dispute.ViolationTicket.OcrViolationTicket = await GetOcrResultsAsync(dispute, cancellationToken);
            }
            catch (Exception ex)
            {
                // Should never reach here in test or prod, but if so then it means the ocr json data is invalid or not parseable by .NET
                // For now, just log the error and return null to mean no image could be found so the GetDispute(id) endpoint doesn't break.
                _logger.LogError(ex, "Could not get violation ticket OCR results for {DisputeId}", dispute.DisputeId);
            }
        }

        return disputes;

    }

    public async Task<long> SaveDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.SaveDisputeAsync(dispute, cancellationToken);
    }

    public async Task<Dispute> GetDisputeAsync(long disputeId, bool isAssign, CancellationToken cancellationToken)
    {
        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, isAssign, cancellationToken);

        dispute.ViolationTicket.OcrViolationTicket = await GetOcrResultsAsync(dispute, cancellationToken);

        // If OcrViolationTicket != null, then this Violation Ticket was scanned using the Azure OCR Form Recognizer at one point.
        // If so, retrieve the image from object storage and return it as well.
        dispute.ViolationTicket.ViolationTicketImage = await GetViolationTicketImageAsync(dispute, cancellationToken);

        return dispute;
    }

    private async Task<OcrViolationTicket?> GetOcrResultsAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        Coms.Client.File? file = await GetFileAsync(dispute, InternalFileProperties.DocumentTypes.OcrResult, cancellationToken);
        if (file is null)
        {
            return null;
        }

        // deserialize
        var result = JsonSerializer.Deserialize<OcrViolationTicket>(file.Data);
        return result;
    }

    /// <summary>
    /// Retrieves a image from the object store with the given imageFilename.
    /// </summary>
    /// <param name="dispute"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<ViolationTicketImage?> GetViolationTicketImageAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        Coms.Client.File? file = await GetFileAsync(dispute, InternalFileProperties.DocumentTypes.TicketImage, cancellationToken);
        if (file is null)
        {
            return null;
        }

        MemoryStream stream = new MemoryStream(); // todo: use memory stream manager
        file.Data.CopyTo(stream);

        return new ViolationTicketImage(stream.ToArray(), file.ContentType ?? "application/octet-stream");
    }


    private async Task<Coms.Client.File?> GetFileAsync(Dispute dispute, string documentType, CancellationToken cancellationToken)
    {
        if (dispute?.NoticeOfDisputeGuid is null)
        {
            _logger.LogInformation("Cannot get dispute {DocumentType}. There is no NoticeOfDisputeGuid", documentType);
            return null;
        }

        if (!Guid.TryParse(dispute.NoticeOfDisputeGuid, out Guid noticeOfDisputeId))
        {
            _logger.LogInformation("Cannot get dispute {DocumentType}. The NoticeOfDisputeGuid value '{NoticeOfDisputeGuid}' is not valid", documentType, dispute.NoticeOfDisputeGuid);
            return null;
        }

        InternalFileProperties properties = new InternalFileProperties
        {
            NoticeOfDisputeId = noticeOfDisputeId,
            DocumentType = documentType
        };

        var metadata = properties.ToMetadata();
        var tags = properties.ToTags();

        FileSearchParameters parameters = new FileSearchParameters(null, metadata, tags);

        IList<FileSearchResult> searchResults = await _objectManagementService.FileSearchAsync(parameters, cancellationToken);

        if (searchResults.Count == 0)
        {
            _logger.LogInformation("Cannot get dispute {DocumentType}. No files found with NoticeOfDisputeGuid value '{NoticeOfDisputeGuid}'", documentType, dispute.NoticeOfDisputeGuid);
            return null;
        }

        FileSearchResult searchResult = searchResults[0];
        if (searchResults.Count > 1)
        {
            // more than one? that is a problem
            _logger.LogInformation("Found {Count} {DocumentType} for {NoticeOfDisputeId}, expected only one. Returning the last created item",
                searchResults.Count, documentType, noticeOfDisputeId);

            searchResult = searchResults.OrderByDescending(_ => _.CreatedAt).First();
        }

        // get the document
        Coms.Client.File file = await _objectManagementService.GetFileAsync(searchResult.Id, cancellationToken);
        return file;
    }


    public async Task<Dispute> UpdateDisputeAsync(long disputeId, Dispute dispute, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.UpdateDisputeAsync(disputeId, dispute, cancellationToken);
    }

    public async Task ValidateDisputeAsync(long disputeId, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        _logger.LogDebug("Dispute status setting to validated");

        Dispute dispute = await _oracleDataApi.ValidateDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            dispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.SVAL,  // Handwritten ticket OCR details validated by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
    }

    public async Task CancelDisputeAsync(long disputeId, string cancelledReason, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        _logger.LogDebug("Dispute cancelled");

        Dispute dispute = await _oracleDataApi.CancelDisputeAsync(disputeId, cancelledReason, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            dispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.SCAN, // Dispute canceled by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish file history of cancellation email
        fileHistoryRecord.AuditLogEntryType = FileHistoryAuditLogEntryType.EMCA;
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish cancel event (consumer(s) will generate email, etc)
        DisputeCancelled cancelledEvent = Mapper.ToDisputeCancelled(dispute);
        await _bus. PublishWithLog(_logger, cancelledEvent, cancellationToken);
    }

    public async Task RejectDisputeAsync(long disputeId, string rejectedReason, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        _logger.LogDebug("Dispute rejected");

        Dispute dispute = await _oracleDataApi.RejectDisputeAsync(disputeId, rejectedReason, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            dispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.SREJ, // Dispute rejected by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        DisputeRejected rejectedEvent = Mapper.ToDisputeRejected(dispute);
        await _bus.PublishWithLog(_logger, rejectedEvent, cancellationToken);
    }

    public async Task SubmitDisputeAsync(long disputeId, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        _logger.LogDebug("Dispute submitted for approval processing");

        // Check for other disputes in processing status with same ticket number
        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, false, cancellationToken);
        string? issuedTime = dispute.IssuedTs is not null ? dispute.IssuedTs.Value.ToString("HH:mm") : "";
        ICollection<DisputeResult> disputeResults = await _oracleDataApi.FindDisputeStatusesAsync(dispute.TicketNumber, null, null, cancellationToken);
        disputeResults = disputeResults.Where(x => x.DisputeStatus == DisputeResultDisputeStatus.PROCESSING).ToList();
        if (disputeResults.Count>0)
        {
            throw new BadHttpRequestException("Another dispute with the same ticket number is currently being processed.");
        }
 
        // Save and status to PROCESSING
        dispute = await _oracleDataApi.SubmitDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            dispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.SPRC, // Dispute submitted to ARC by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // publish file history of email sent
        fileHistoryRecord.AuditLogEntryType = FileHistoryAuditLogEntryType.EMCF;
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish submit event (consumer(s) will push event to ARC and generate email)
        DisputeApproved approvedEvent = Mapper.ToDisputeApproved(dispute);
        await _bus.PublishWithLog(_logger, approvedEvent, cancellationToken);
    }

    public async Task DeleteDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        await _oracleDataApi.DeleteDisputeAsync(disputeId, cancellationToken);
    }

    public async Task<string> ResendEmailVerificationAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Email verification sent");

        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, false, cancellationToken);

        // Publish a message to resend email verification email (the event will be picked up by the saga to generate email, etc)
        var message = new ResendEmailVerificationEmail { NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid) };
        await _bus.PublishWithLog(_logger, message, cancellationToken);
        return "Email verification sent";
    }

    /// <summary>
    /// Accepts a citizen's requested changes to their Disputant Contact information.
    /// </summary>
    /// <param name="updateStatusId"></param>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AcceptDisputeUpdateRequestAsync(long updateStatusId, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        // TCVP-1975 - consumers of this message are expected to:
        // - call oracle-data-api to patch the Dispute with the DisputeUpdateRequest changes.
        // - call oracle-data-api to update request status in OCCAM.
        // - send confirmation email indicating request was accepted
        // - populate file/email history records
        DisputeUpdateRequestAccepted message = new(updateStatusId, GetUserName(user));
        await _bus.PublishWithLog(_logger, message, cancellationToken);
    }

    /// <summary>
    /// Rejects a citizen's requested changes to their Disputant Contact information.
    /// </summary>
    /// <param name="updateStatusId"></param>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RejectDisputeUpdateRequestAsync(long updateStatusId, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        // TCVP-1974 - consumers of this message are expected to:
        // - call oracle-data-api to update request status in OCCAM.
        // - send confirmation email indicating request was rejected
        // - populate file/email history records
        DisputeUpdateRequestRejected message = new(updateStatusId, GetUserName(user));
        await _bus.PublishWithLog(_logger, message, cancellationToken);
    }

    /// <summary>
    /// Returns a list of all disputes with pending update requests.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ICollection<DisputeWithUpdates>> GetAllDisputesWithPendingUpdateRequestsAsync(CancellationToken cancellationToken)
    {
        ICollection<DisputeWithUpdates> disputesWithUpdates = new Collection<DisputeWithUpdates>();
        ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest> pendingDisputeUpdateRequests = await _oracleDataApi.GetDisputeUpdateRequestsAsync(null, Status.PENDING, cancellationToken);

        foreach (TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest disputeUpdateRequest in pendingDisputeUpdateRequests)
        {
            DisputeWithUpdates? disputeWithUpdates = new DisputeWithUpdates();
            if (disputesWithUpdates.FirstOrDefault(x => x.DisputeId == disputeUpdateRequest.DisputeId) is null)
            {
                try
                {
                    Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeUpdateRequest.DisputeId, false, cancellationToken);

                    // Fill in record to return
                    disputeWithUpdates.DisputeId = dispute.DisputeId;
                    disputeWithUpdates.DisputantGivenName1 = dispute.DisputantGivenName1;
                    disputeWithUpdates.DisputantGivenName2 = dispute.DisputantGivenName2;
                    disputeWithUpdates.DisputantGivenName3 = dispute.DisputantGivenName3;
                    disputeWithUpdates.DisputantSurname = dispute.DisputantSurname;
                    disputeWithUpdates.UserAssignedTo = dispute.UserAssignedTo;
                    disputeWithUpdates.UserAssignedTs = dispute.UserAssignedTs;
                    disputeWithUpdates.Status = dispute.Status;
                    disputeWithUpdates.TicketNumber = dispute.TicketNumber;
                    disputeWithUpdates.SubmittedTs = dispute.SubmittedTs;
                    disputeWithUpdates.EmailAddress = dispute.EmailAddress;
                    disputeWithUpdates.EmailAddressVerified = dispute.EmailAddressVerified; 

                    // Check for future court hearing date
                    disputeWithUpdates.HearingDate = null;
                    ICollection<JJDispute> jjDisputes = await _oracleDataApi.GetJJDisputesAsync(null, dispute.TicketNumber, cancellationToken);
                    if (jjDisputes != null && jjDisputes.Count > 0)
                    {
                        // review first one
                        foreach(var jjDispute in jjDisputes)
                        {
                            if (jjDispute.JjDisputeCourtAppearanceRoPs.Count() > 0)
                            {
                                foreach (var courtAppearance in jjDispute.JjDisputeCourtAppearanceRoPs)
                                {
                                    if (courtAppearance.AppearanceTs > DateTimeOffset.Now && (disputeWithUpdates.HearingDate is null || disputeWithUpdates.HearingDate > courtAppearance.AppearanceTs))
                                    {
                                        disputeWithUpdates.HearingDate = courtAppearance.AppearanceTs;
                                    }
                                }

                            }
                        }
                    }

                    disputesWithUpdates.Add(disputeWithUpdates);
                }
                catch {  
                    // dont crash carry on
                }
            } else
            {
                disputeWithUpdates = disputesWithUpdates.FirstOrDefault(x => x.DisputeId == disputeUpdateRequest.DisputeId);
            }
            // check whether this udpate request is for an adjournment document
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            disputeWithUpdates.AdjournmentDocument = false;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (disputeUpdateRequest.UpdateType == DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT)
            {
                DocumentUpdateJSON? documentUpdateJSON = JsonSerializer.Deserialize<DocumentUpdateJSON>(disputeUpdateRequest.UpdateJson);
                if (documentUpdateJSON is not null && documentUpdateJSON.DocumentType == "Application for Adjournment") 
                {
                    disputeWithUpdates.AdjournmentDocument = true;
                }
            }

            // check whether this update request is for a change of plea
            disputeWithUpdates.ChangeOfPlea = false;
            if (disputeUpdateRequest.UpdateType == DisputeUpdateRequestUpdateType.COUNT)
            {
                CountUpdateJSON? countUpdateJSON = JsonSerializer.Deserialize<CountUpdateJSON>(disputeUpdateRequest.UpdateJson);
                if (countUpdateJSON is not null && countUpdateJSON.pleaCode is not null)
                {
                    disputeWithUpdates.ChangeOfPlea = true;
                }
            }
        }

        return disputesWithUpdates;

    }

    /// <summary>
    /// Returns a list of all dispute update requests for a given dispute id.
    /// </summary>
    /// <param name="disputeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ICollection<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long disputeId, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetDisputeUpdateRequestsAsync(disputeId, null, cancellationToken);
    }

    private string GetUserName(ClaimsPrincipal user) => user.Identity?.Name ?? string.Empty;
}
