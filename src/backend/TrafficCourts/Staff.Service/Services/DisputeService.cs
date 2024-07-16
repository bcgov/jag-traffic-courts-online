using MassTransit;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Text.Json;
using TrafficCourts.Collections;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Coms.Client;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Caching;
using TrafficCourts.Staff.Service.Mappers;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Models.Disputes;
using OcrViolationTicket = TrafficCourts.Domain.Models.OcrViolationTicket;
using ZiggyCreatures.Caching.Fusion;
using MediatR;
using TrafficCourts.Domain.Events;
using TrafficCourts.TicketSearch;
using Microsoft.Extensions.Logging;
using System.Threading;
using TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

namespace TrafficCourts.Staff.Service.Services;

public record GetDisputeOptions
{
    public long DisputeId { get; init; }

    public bool Assign { get; init; }

    /// <summary>
    /// If true, the service will attempt to retrieve the Disputant's name from ICBC.
    /// </summary>
    public bool GetNameFromIcbc { get; init; }
}

/// <summary>
/// Summary description for Class1
/// </summary>
public class DisputeService : IDisputeService, 
    INotificationHandler<DisputeCreatedEvent>,
    INotificationHandler<DisputeChangedEvent>,
    INotificationHandler<DisputesAssignedEvent>,
    INotificationHandler<DisputesUnassignedEvent>
{
    private readonly IAgencyLookupService _agencyLookupService;
    private readonly IBus _bus;
    private readonly IFusionCache _cache;
    private readonly ILogger<DisputeService> _logger;
    private readonly IObjectManagementService _objectManagementService;
    private readonly IOracleDataApiService _oracleDataApi;
    private readonly IProvinceLookupService _provinceLookupService;
    private readonly IStaffDocumentService _documentService;
    private readonly ITicketSearchService _ticketSearchService;

    public DisputeService(
        IOracleDataApiService oracleDataApi,
        IBus bus,
        IObjectManagementService objectManagementService,
        IAgencyLookupService agencyLookupService,
        IProvinceLookupService provinceLookupService,
        IStaffDocumentService documentService,
        ITicketSearchService ticketSearchService,
        IFusionCache cache,
        ILogger<DisputeService> logger)
    {
        _agencyLookupService = agencyLookupService ?? throw new ArgumentNullException(nameof(agencyLookupService));
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _objectManagementService = objectManagementService ?? throw new ArgumentNullException(nameof(objectManagementService));
        _provinceLookupService = provinceLookupService ?? throw new ArgumentNullException(nameof(provinceLookupService));
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        _ticketSearchService = ticketSearchService ?? throw new ArgumentNullException(nameof(ticketSearchService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<DisputeListItem>> GetAllDisputesAsync(ExcludeStatus? excludeStatus, CancellationToken cancellationToken)
    {
        List<DisputeListItem> disputes = await GetCachedDisputesAsync(cancellationToken);

        if (excludeStatus is not null)
        {
            GetAllDisputesParameters filter = new() { ExcludeStatus = [excludeStatus.Value] };
            disputes = disputes.Filter(filter).ToList();
        }

        return disputes;
    }

    public async Task<GetDisputeCountResponse> GetDisputeCountAsync(DisputeStatus status, CancellationToken cancellationToken)
    {
        GetAllDisputesParameters filter = new() { Status = [status] };

        var disputes = await GetCachedDisputesAsync(cancellationToken);

        var count = disputes.Filter(filter).Count();

        return new GetDisputeCountResponse(status, count);
    }

    public async Task<PagedDisputeListItemCollection> GetAllDisputesAsync(GetAllDisputesParameters? parameters, CancellationToken cancellationToken)
    {
        var disputes = await GetCachedDisputesAsync(cancellationToken);

        // apply default sort if none supplied
        parameters ??= new GetAllDisputesParameters();
        parameters.SetDefaultSortIfNotSpecified();

        var agencies = await _agencyLookupService.GetListAsync();

        // apply fitler, sorting and paging
        var paged = disputes
            .Filter(parameters, agencies)
            .Sort(parameters)
            .Page(parameters, 25);

        return new PagedDisputeListItemCollection(paged);
    }

    public async Task<long> SaveDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.SaveDisputeAsync(dispute, cancellationToken);
    }

    public async Task<Dispute> GetDisputeAsync(GetDisputeOptions options, CancellationToken cancellationToken)
    {
        Dispute dispute = await _oracleDataApi.GetDisputeAsync(options.DisputeId, options.Assign, cancellationToken);

        await GetOcrImageAndResults(dispute, cancellationToken);
        await GetFileAttachments(dispute, cancellationToken);
        await GetIcbcTicketInformation(dispute, options, cancellationToken);

        return dispute;
    }

    public async Task<Dispute> GetDisputeAsync(long disputeId, bool isAssign, CancellationToken cancellationToken)
    {
        var options = new GetDisputeOptions { DisputeId = disputeId, Assign = isAssign };

        Dispute dispute = await GetDisputeAsync(options, cancellationToken);

        return dispute;
    }

    public async Task<Dispute> UpdateDisputeAsync(long disputeId, ClaimsPrincipal user, string? staffComment, Dispute dispute, CancellationToken cancellationToken)
    {
        Dispute updatedDispute = await _oracleDataApi.UpdateDisputeAsync(disputeId, dispute, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            updatedDispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.FRMK, // VTC staff has added a file remark for saving or updating a dispute in Ticket Validation
            GetUserName(user),
            staffComment!);

        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return updatedDispute;
    }

    public async Task ValidateDisputeAsync(long disputeId, Dispute? dispute, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (dispute != null)
        {
            _logger.LogDebug("Saving dispute before validating");
            _ = await _oracleDataApi.UpdateDisputeAsync(disputeId, dispute, cancellationToken);
        }

        _logger.LogDebug("Dispute status setting to validated");

        Dispute validatedDispute = await _oracleDataApi.ValidateDisputeAsync(disputeId, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            validatedDispute.NoticeOfDisputeGuid,
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

        // Publish file history for cancelled remarks
        SaveFileHistoryRecord fileHistoryRecordRemark = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            dispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.FRMK,
            GetUserName(user),
            cancelledReason);
        await _bus.PublishWithLog(_logger, fileHistoryRecordRemark, cancellationToken);

        // Publish cancel event (consumer(s) will generate email, etc)
        DisputeCancelled cancelledEvent = Mapper.ToDisputeCancelled(dispute);
        await _bus.PublishWithLog(_logger, cancelledEvent, cancellationToken);
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

        // Publish file history for rejected remarks
        SaveFileHistoryRecord fileHistoryRecordRemark = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            dispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.FRMK,
            GetUserName(user),
            rejectedReason);
        await _bus.PublishWithLog(_logger, fileHistoryRecordRemark, cancellationToken);

        // Publish reject event (consumer(s) will generate email, etc)
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
        ICollection<DisputeResult> disputeResults = await _oracleDataApi.FindDisputeStatusesAsync(dispute.TicketNumber, null, null, null, cancellationToken);
        disputeResults = disputeResults.Where(x => x.DisputeStatus == DisputeResultDisputeStatus.PROCESSING).ToList();
        if (disputeResults.Count>0)
        {
            throw new BadHttpRequestException("Another dispute with the same ticket number is currently being processed.");
        }
 
        // Status to PROCESSING
        dispute = await _oracleDataApi.SubmitDisputeAsync(disputeId, cancellationToken);

        // set AddressProvince to 2 character abbreviation code if prov seq no & ctry id present
        if (dispute.AddressProvinceSeqNo != null)
        {
            var provFound = await _provinceLookupService.GetByProvSeqNoCtryIdAsync(dispute.AddressProvinceSeqNo.ToString(), dispute.AddressCountryId.ToString());
            if (provFound != null) { dispute.AddressProvince = provFound.ProvAbbreviationCd; }
        }

        // Publish submit event (consumer(s) will push event to ARC and generate email)
        DisputeApproved approvedEvent = Mapper.ToDisputeApproved(dispute);
        await _bus.PublishWithLog(_logger, approvedEvent, cancellationToken);

        // Publish file history
        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithNoticeOfDisputeId(
            dispute.NoticeOfDisputeGuid,
            FileHistoryAuditLogEntryType.SPRC, // Dispute submitted to ARC by staff
            GetUserName(user));
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        // publish file history of email sent
        fileHistoryRecord.AuditLogEntryType = FileHistoryAuditLogEntryType.EMCF;
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
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

        var message = new RequestEmailVerification { 
            NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
            TicketNumber = dispute.TicketNumber,
            EmailAddress = dispute.EmailAddress,
            IsUpdateEmailVerification = true
        };
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
        ICollection<Domain.Models.DisputeUpdateRequest> pendingDisputeUpdateRequests = await _oracleDataApi.GetDisputeUpdateRequestsAsync(null, Status.PENDING, cancellationToken);

        Dictionary<long, DisputeWithUpdates> disputesWithUpdates = [];

        foreach (Domain.Models.DisputeUpdateRequest disputeUpdateRequest in pendingDisputeUpdateRequests.OrderBy(_ => _.DisputeId))
        {
            // check if we have already loaded this dispute
            if (!disputesWithUpdates.TryGetValue(disputeUpdateRequest.DisputeId, out DisputeWithUpdates? disputeWithUpdates))
            {
                try
                {

                    Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeUpdateRequest.DisputeId, false, cancellationToken);

                    // Fill in record to return
                    disputeWithUpdates = new DisputeWithUpdates
                    {
                        DisputeId = dispute.DisputeId,
                        DisputantGivenName1 = dispute.DisputantGivenName1,
                        DisputantGivenName2 = dispute.DisputantGivenName2,
                        DisputantGivenName3 = dispute.DisputantGivenName3,
                        DisputantSurname = dispute.DisputantSurname,
                        UserAssignedTo = dispute.UserAssignedTo,
                        UserAssignedTs = dispute.UserAssignedTs,
                        Status = dispute.Status,
                        TicketNumber = dispute.TicketNumber,
                        SubmittedTs = dispute.SubmittedTs,
                        EmailAddress = dispute.EmailAddress,
                        EmailAddressVerified = dispute.EmailAddressVerified
                    };

                    await SetHearingDateAsync(disputeWithUpdates, cancellationToken);

                    disputesWithUpdates.Add(disputeWithUpdates.DisputeId, disputeWithUpdates);
                }
                catch (Exception exception)
                {
                    // dont crash carry on
                    _logger.LogInformation(exception, "Error getting pending dispute for {DisputeId}", disputeUpdateRequest.DisputeId);
                }
            }

            SetAdjournmentDocumentFlag(disputeWithUpdates, disputeUpdateRequest);
            SetChangeOfPleaFlag(disputeWithUpdates, disputeUpdateRequest);
        }

        var result = disputesWithUpdates.Values.ToList();
        return result;
    }

    private async Task SetHearingDateAsync(DisputeWithUpdates dispute, CancellationToken cancellationToken)
    {
        // Check for future court hearing date
        dispute.HearingDate = null;

        ICollection<JJDispute> jjDisputes = await _oracleDataApi.GetJJDisputesAsync(null, dispute.TicketNumber, cancellationToken);

        if (jjDisputes is not null)
        {
            var now = DateTimeOffset.Now;

            // find the first future court appearance date that is after right now
            dispute.HearingDate = jjDisputes
                .SelectMany(_ => _.JjDisputeCourtAppearanceRoPs)
                .Where(_ => _.AppearanceTs > now)
                .OrderBy(_ => _.AppearanceTs)
                .Select(_ => _.AppearanceTs)
                .FirstOrDefault();
        }
    }

    private void SetAdjournmentDocumentFlag(DisputeWithUpdates? dispute, Domain.Models.DisputeUpdateRequest disputeUpdateRequest)
    {
        if (dispute is null)
        {
            return;
        }

        // check whether this udpate request is for an adjournment document
        dispute.AdjournmentDocument = false;
        if (disputeUpdateRequest.UpdateType == DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT)
        {
            DocumentUpdateJSON? documentUpdateJSON = JsonSerializer.Deserialize<DocumentUpdateJSON>(disputeUpdateRequest.UpdateJson, ModelJsonSerializerContext.Default.DocumentUpdateJSON);
            if (documentUpdateJSON is not null && documentUpdateJSON.DocumentType == "Application for Adjournment")
            {
                dispute.AdjournmentDocument = true;
            }
        }
    }

    private void SetChangeOfPleaFlag(DisputeWithUpdates? dispute, Domain.Models.DisputeUpdateRequest disputeUpdateRequest)
    {
        if (dispute is null)
        {
            return;
        }

        // check whether this update request is for a change of plea
        dispute.ChangeOfPlea = false;
        if (disputeUpdateRequest.UpdateType == DisputeUpdateRequestUpdateType.COUNT)
        {
            CountUpdateJSON? countUpdateJSON = JsonSerializer.Deserialize<CountUpdateJSON>(disputeUpdateRequest.UpdateJson, ModelJsonSerializerContext.Default.CountUpdateJSON);
            if (countUpdateJSON is not null && countUpdateJSON.pleaCode is not null)
            {
                dispute.ChangeOfPlea = true;
            }
        }
    }

    /// <summary>
    /// Returns a list of all dispute update requests for a given dispute id.
    /// </summary>
    /// <param name="disputeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ICollection<Domain.Models.DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long disputeId, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetDisputeUpdateRequestsAsync(disputeId, null, cancellationToken);
    }

    public async Task Handle(DisputeCreatedEvent notification, CancellationToken cancellationToken)
    {
        await ClearCachedDisputeListItemsAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task Handle(DisputeChangedEvent notification, CancellationToken cancellationToken)
    {
        await ClearCachedDisputeListItemsAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task Handle(DisputesAssignedEvent notification, CancellationToken cancellationToken)
    {
        await ClearCachedDisputeListItemsAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task Handle(DisputesUnassignedEvent notification, CancellationToken cancellationToken)
    {
        await ClearCachedDisputeListItemsAsync(cancellationToken).ConfigureAwait(false);
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

    private async Task GetOcrImageAndResults(Dispute dispute, CancellationToken cancellationToken)
    {
        dispute.ViolationTicket.OcrViolationTicket = await GetOcrResultsAsync(dispute, cancellationToken);

        // If OcrViolationTicket != null, then this Violation Ticket was scanned using the Azure OCR Form Recognizer at one point.
        // If so, retrieve the image from object storage and return it as well.
        dispute.ViolationTicket.ViolationTicketImage = await GetViolationTicketImageAsync(dispute, cancellationToken);
    }

    private async Task GetFileAttachments(Dispute dispute, CancellationToken cancellationToken)
    {
        List<FileMetadata>? disputeFiles = null;

        // search by notice of dispute guid
        if (dispute.NoticeOfDisputeGuid is not null && Guid.TryParse(dispute.NoticeOfDisputeGuid, out Guid noticeOfDisputeId))
        {
            // create new search properties
            DocumentProperties properties = new DocumentProperties { NoticeOfDisputeId = noticeOfDisputeId };
            disputeFiles = await _documentService.FindFilesAsync(properties, cancellationToken);
        }

        dispute.FileData = disputeFiles;

        // TCVP-2878 Filter files that are corrupt in COMS (missing attributes)
        if (dispute.FileData is not null)
        {
            int count = dispute.FileData.Count;
            // If there is a missing fileName, remove it from the list as we can't display such an object in the UI.
            dispute.FileData = dispute.FileData.Where(x => x.FileName is not null).ToList();

            if (count != dispute.FileData.Count)
            {
                // This should never happen, but if it does, it means that there is bad data in COMS (an application error)
                _logger.LogError("COMS has files with missing filenames (bad data). Excluded {count} files from search results", count - dispute.FileData.Count);
            }
        }
    }

    private async Task GetIcbcTicketInformation(Dispute dispute, GetDisputeOptions options, CancellationToken cancellationToken)
    {
        if (options.GetNameFromIcbc && dispute.IssuedTs is not null)
        {
            TimeOnly timeOfDay = TimeOnly.FromTimeSpan(dispute.IssuedTs.Value.TimeOfDay);
            try
            {
                Ticket? ticket = await _ticketSearchService.SearchAsync(dispute.TicketNumber, timeOfDay, cancellationToken);
                if (ticket is not null)
                {
                    dispute.IcbcName = new IcbcNameDetail(ticket.Surname, ticket.FirstGivenName, ticket.SecondGivenName);
                }
            }
            catch (TicketSearchErrorException exception)
            {
                _logger.LogInformation(exception, "Cannot get ICBC name for dispute {DisputeId}", dispute.DisputeId);
            }
        }
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

        InternalFileProperties properties = new()
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

    private string GetUserName(ClaimsPrincipal user) => user.Identity?.Name ?? string.Empty;

    private async Task<List<DisputeListItem>> GetCachedDisputesAsync(CancellationToken cancellationToken)
    {
        var key = Cache.OracleData.DisputeListItems();

        var value = await _cache.GetOrSetAsync<List<DisputeListItem>>(
            key,
            ct => GetAllDisputesAsListAsync(ct),
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return value;
    }

    private async Task<List<DisputeListItem>> GetAllDisputesAsListAsync(CancellationToken cancellationToken)
    {
        ICollection<DisputeListItem> values = await _oracleDataApi.GetAllDisputesAsync(null, null, cancellationToken);

        if (values is List<DisputeListItem> listOfDisputes)
        {
            return listOfDisputes;
        }

        return new List<DisputeListItem>(values);
    }

    private async Task ClearCachedDisputeListItemsAsync(CancellationToken cancellationToken)
    {
        var key = Cache.OracleData.DisputeListItems();
        await _cache.RemoveAsync(key, token: cancellationToken);

    }
}
