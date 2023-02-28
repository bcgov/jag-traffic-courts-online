using MassTransit;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;
using System.Text.Json;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Staff.Service.Models;
using System.Collections.ObjectModel;
using System.Security.Claims;

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
    private readonly IFilePersistenceService _filePersistenceService;

    public DisputeService(
        IOracleDataApiClient oracleDataApi,
        IBus bus,
        IFilePersistenceService filePersistenceService,
        ILogger<DisputeService> logger,
        ICancelledDisputeEmailTemplate cancelledDisputeEmailTemplate,
        IRejectedDisputeEmailTemplate rejectedDisputeEmailTemplate)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cancelledDisputeEmailTemplate = cancelledDisputeEmailTemplate;
        _rejectedDisputeEmailTemplate = rejectedDisputeEmailTemplate;
    }

    public async Task<ICollection<Dispute>> GetAllDisputesAsync(ExcludeStatus? excludeStatus, CancellationToken cancellationToken)
    {
        ICollection<Dispute> disputes = await _oracleDataApi.GetAllDisputesAsync(null, excludeStatus, cancellationToken);

        foreach(Dispute dispute in disputes)
        {
            try
            {
                // When reviewing the list of tickets, the JSON is needed to compute SystemDetectedOCRIssues
                OcrViolationTicket? ocrViolationTicket = null;
                if (!string.IsNullOrEmpty(dispute.OcrTicketFilename))
                {
                    // Retrieve deserialized OCR Violation Ticket JSON Data from object storage for the given filename (NoticeOfDisputeGuid)
                    ocrViolationTicket = await _filePersistenceService.GetJsonDataAsync<OcrViolationTicket>(dispute.OcrTicketFilename, cancellationToken);
                    dispute.ViolationTicket.OcrViolationTicket = ocrViolationTicket;
                }
            }
            catch (Exception ex)
            {
                // Should never reach here in test or prod, but if so then it means the ocr json data is invalid or not parseable by .NET
                // For now, just log the error and return null to mean no image could be found so the GetDispute(id) endpoint doesn't break.
                _logger.LogError(ex, "Could not extract object store file reference from json data while retrieving disputes.");
            }
        }

        return disputes;

    }

    public async Task<long> SaveDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.SaveDisputeAsync(dispute, cancellationToken);
    }

    public async Task<Dispute> GetDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, cancellationToken);

        OcrViolationTicket? ocrViolationTicket = null;
        if (!string.IsNullOrEmpty(dispute.OcrTicketFilename))
        {
            // Retrieve deserialized OCR Violation Ticket JSON Data from object storage for the given filename (NoticeOfDisputeGuid)
            ocrViolationTicket = await _filePersistenceService.GetJsonDataAsync<OcrViolationTicket>(dispute.OcrTicketFilename, cancellationToken);
            dispute.ViolationTicket.OcrViolationTicket = ocrViolationTicket;
        }

        // If OcrViolationTicket != null, then this Violation Ticket was scanned using the Azure OCR Form Recognizer at one point.
        // If so, retrieve the image from object storage and return it as well.
        if (ocrViolationTicket != null)
        {
            dispute.ViolationTicket.ViolationTicketImage = await GetViolationTicketImageAsync(ocrViolationTicket.ImageFilename, cancellationToken);
        }

        return dispute;
    }

    /// <summary>
    /// Extracts the path to the image located in the object store from the JSON string, or null if not filename could be found.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public string? GetViolationTicketImageFilename(string json)
    {
        if (json is not null)
        {
            try
            {
                JsonElement element = JsonSerializer.Deserialize<JsonElement>(json);
                if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("ImageFilename", out JsonElement imageFilename))
                {
                    return imageFilename.GetString();
                }
            }
            catch (Exception ex)
            {
                // Should never reach here in test or prod, but if so then it means the ocr json data is invalid or not parseable by .NET
                // For now, just log the error and return null to mean no image could be found so the GetDispute(id) endpoint doesn't break.
                _logger.LogError(ex, "Could not extract object store file reference from json data");
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieves a image from the object store with the given imageFilename.
    /// </summary>
    /// <param name="imageFilename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<ViolationTicketImage?> GetViolationTicketImageAsync(string? imageFilename, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(imageFilename))
        {
            return null;
        }

        using (_logger.BeginScope(new Dictionary<string, object> { ["FileName"] = imageFilename }))
        {
            try
            {
                MemoryStream stream = await _filePersistenceService.GetFileAsync(imageFilename, cancellationToken);
                FileMimeType? mimeType = stream.GetMimeType();
                if (mimeType is null)
                {
                    _logger.LogWarning("Could not determine mime type for file");
                    return null;
                }

                return new ViolationTicketImage(stream.ToArray(), mimeType.MimeType);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not retrieve image from object storage");
                throw;
            }
        }
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
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
            dispute.DisputeId,
            FileHistoryAuditLogEntryType.SVAL,  // Handwritten ticket OCR details validated by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
    }

    public async Task CancelDisputeAsync(long disputeId, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        _logger.LogDebug("Dispute cancelled");

        Dispute dispute = await _oracleDataApi.CancelDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
            dispute.DisputeId,
            FileHistoryAuditLogEntryType.SCAN, // Dispute canceled by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        DisputeCancelled cancelledEvent = Mapper.ToDisputeCancelled(dispute);
        await _bus.PublishWithLog(_logger, cancelledEvent, cancellationToken);

        var emailMessage = _cancelledDisputeEmailTemplate.Create(dispute);
        await _bus.PublishWithLog(_logger, emailMessage, cancellationToken);
    }

    public async Task RejectDisputeAsync(long disputeId, string rejectedReason, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        _logger.LogDebug("Dispute rejected");

        Dispute dispute = await _oracleDataApi.RejectDisputeAsync(disputeId, rejectedReason, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
            dispute.DisputeId,
            FileHistoryAuditLogEntryType.SREJ, // Dispute rejected by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        DisputeRejected rejectedEvent = Mapper.ToDisputeRejected(dispute);
        await _bus.PublishWithLog(_logger, rejectedEvent, cancellationToken);

        var emailMessage = _rejectedDisputeEmailTemplate.Create(dispute);
        await _bus.PublishWithLog(_logger, emailMessage, cancellationToken);
    }

    public async Task SubmitDisputeAsync(long disputeId, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        _logger.LogDebug("Dispute submitted for approval processing");

        // Save and status to PROCESSING
        Dispute dispute = await _oracleDataApi.SubmitDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
            dispute.DisputeId,
            FileHistoryAuditLogEntryType.SPRC, // Dispute submitted to ARC by staff
            GetUserName(user));
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

        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, cancellationToken);

        // Publish submit event (consumer(s) will generate email, etc)
        EmailVerificationSend emailVerificationSentEvent = Mapper.ToEmailVerification(new Guid(dispute.NoticeOfDisputeGuid));
        await _bus.PublishWithLog(_logger, emailVerificationSentEvent, cancellationToken);

        return "Email verification sent";
    }

    /// <summary>
    /// Accepts a citizen's requested changes to their Disputant Contact information.
    /// </summary>
    /// <param name="updateStatusId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AcceptDisputeUpdateRequestAsync(long updateStatusId, CancellationToken cancellationToken)
    {
        // TCVP-1975 - consumers of this message are expected to:
        // - call oracle-data-api to patch the Dispute with the DisputeUpdateRequest changes.
        // - call oracle-data-api to update request status in OCCAM.
        // - send confirmation email indicating request was accepted
        // - populate file/email history records
        DisputeUpdateRequestAccepted message = new(updateStatusId);
        await _bus.PublishWithLog(_logger, message, cancellationToken);
    }

    /// <summary>
    /// Rejects a citizen's requested changes to their Disputant Contact information.
    /// </summary>
    /// <param name="updateStatusId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task RejectDisputeUpdateRequestAsync(long updateStatusId, CancellationToken cancellationToken)
    {
        // TCVP-1974 - consumers of this message are expected to:
        // - call oracle-data-api to update request status in OCCAM.
        // - send confirmation email indicating request was rejected
        // - populate file/email history records
        DisputeUpdateRequestRejected message = new(updateStatusId);
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
                    Dispute dispute = await _oracleDataApi.GetDisputeNoAssignAsync(disputeUpdateRequest.DisputeId, cancellationToken);

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
