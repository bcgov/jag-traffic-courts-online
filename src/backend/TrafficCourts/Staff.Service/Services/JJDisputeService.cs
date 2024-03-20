using MassTransit;
using System.Security.Claims;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.OpenAPIs.Keycloak;
using TrafficCourts.Common.OpenAPIs.Keycloak.v22_0;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v22_0;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;
using TrafficCourts.Logging;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;
using JJDisputeStatus = TrafficCourts.Domain.Models.JJDisputeStatus;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public partial class JJDisputeService : IJJDisputeService
{
    private readonly IOracleDataApiService _oracleDataApi;
    private readonly IBus _bus;
    private readonly IStaffDocumentService _documentService;
    private readonly IKeycloakService _keycloakService;
    private readonly IStatuteLookupService _lookupService;
    private readonly ILogger<JJDisputeService> _logger;

    public JJDisputeService(IOracleDataApiService oracleDataApi, IBus bus, IStaffDocumentService comsService, IKeycloakService keycloakService, IStatuteLookupService lookupService, ILogger<JJDisputeService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _documentService = comsService ?? throw new ArgumentNullException(nameof(comsService));
        _keycloakService = keycloakService ?? throw new ArgumentNullException(nameof(keycloakService));
        _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<JJDispute>> GetAllJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetJJDisputesAsync(jjAssignedTo, null, cancellationToken);
    }

    public async Task<JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.GetJJDisputeAsync(ticketNumber, assignVTC, cancellationToken);

        // Search by dispute id
        Domain.Models.DocumentProperties properties = new() { TcoDisputeId = dispute.Id };

        List<TrafficCourts.Domain.Models.FileMetadata> disputeFiles = await _documentService.FindFilesAsync(properties, cancellationToken);

        // search by notice of dispute guid
        if (dispute.NoticeOfDisputeGuid is not null && Guid.TryParse(dispute.NoticeOfDisputeGuid, out Guid noticeOfDisputeId))
        {
            // create new search properties
            properties = new Domain.Models.DocumentProperties { NoticeOfDisputeId = noticeOfDisputeId };
            List<TrafficCourts.Domain.Models.FileMetadata> files = await _documentService.FindFilesAsync(properties, cancellationToken);
            AddUnique(disputeFiles, files);
        }

        dispute.FileData = disputeFiles;

        // TCVP-2792 Filter files and exclude citizen-uploaded documents whose status is not ACCEPTED if assignVTC=false (requests coming from the jj workbench have this set to false, staff workbench has it set to true)
        if (!assignVTC && dispute.FileData is not null) {
            dispute.FileData = dispute.FileData.Where(x => x.DocumentSource != TrafficCourts.Domain.Models.DocumentSource.Citizen || x.DocumentStatus == DisputeUpdateRequestStatus.ACCEPTED.ToString()).ToList();
        }

        // Populate the statute description of each count of the JJDispute
        foreach (var count in dispute.JjDisputedCounts)
        {
            // Set the full description back to the JJDisputedCount as part of the JJDispute to be returned
            count.Description = await GetStatuteDescriptionAsync(count);
        }

        return dispute;
    }

    public async Task<TicketImageDataJustinDocument> GetJustinDocumentAsync(string ticketNumber, DocumentType documentType, CancellationToken cancellationToken)
    {
        TicketImageDataJustinDocument justinDocument = await _oracleDataApi.GetTicketImageDataAsync(ticketNumber, documentType, cancellationToken);

        return justinDocument;
    }

    public async Task<JJDispute> UpdateJJDisputeCascadeAsync(JJDispute jjDispute, ClaimsPrincipal user, CancellationToken cancellationToken) {
        JJDispute dispute = await _oracleDataApi.UpdateJJDisputeCascadeAsync(jjDispute.TicketNumber, true, jjDispute, cancellationToken);

        // TCVP-2522 save FileHistory
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.TicketNumber = jjDispute.TicketNumber;
        fileHistoryRecord.AuditLogEntryType = FileHistoryAuditLogEntryType.SADM; // Dispute updated by Support Admin Staff 
        fileHistoryRecord.ActionByApplicationUser = GetUserName(user);
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> SubmitAdminResolutionAsync(long disputeId, bool checkVTC, JJDispute jjDispute, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.UpdateJJDisputeAsync(jjDispute.TicketNumber, checkVTC, jjDispute, cancellationToken);

        // Populate the statute description of each count of the JJDispute
        foreach (var count in dispute.JjDisputedCounts)
        {
            // Set the full description back to the JJDisputedCount as part of the JJDispute to be returned
            count.Description = await GetStatuteDescriptionAsync(count);
        }

        if (dispute.Status == JJDisputeStatus.IN_PROGRESS)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
                jjDispute.TicketNumber,
                FileHistoryAuditLogEntryType.JPRG, // Dispute decision details saved for later
                GetUserName(user));
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }
        else if (dispute.Status == JJDisputeStatus.CONFIRMED)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
                jjDispute.TicketNumber,
                FileHistoryAuditLogEntryType.JCNF, // Dispute decision confirmed/submitted by JJ
                GetUserName(user));
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }

        return dispute;
    }

    public async Task AssignJJDisputesToJJ(List<string> ticketNumbers, string? username, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        await _oracleDataApi.AssignJJDisputesToJJAsync(ticketNumbers, username, cancellationToken);

        // Publish file history
        foreach (string ticketNumber in ticketNumbers)
        {
            JJDispute dispute = await _oracleDataApi.GetJJDisputeAsync(ticketNumber, false, cancellationToken);

            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
                dispute.TicketNumber,
                FileHistoryAuditLogEntryType.JASG, // Dispute assigned to JJ
                GetUserName(user));

            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }
    }

    public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, string remark, bool checkVTC, ClaimsPrincipal user, bool recalled, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ReviewJJDisputeAsync(ticketNumber, checkVTC, recalled, remark, cancellationToken);

        FileHistoryAuditLogEntryType fileHistoryType = FileHistoryAuditLogEntryType.VREV; // Dispute returned to JJ for review

        if (dispute.Recalled == true)
        {
            fileHistoryType = FileHistoryAuditLogEntryType.RCLD; // Dispute recalled by JJ
        }

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
            dispute.TicketNumber,
            fileHistoryType,
            GetUserName(user));

        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string? remark, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        try
        {
            JJDispute dispute = await _oracleDataApi.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, cancellationToken);

            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
                dispute.TicketNumber,
                FileHistoryAuditLogEntryType.JDIV, 
                GetUserName(user)); // Dispute change of plea required / Divert to court appearance

            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

            return dispute;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "could not set status to require court hearing");
            throw;
        }

    }

    public async Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTC, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        // Get the assigned JJ's PartId from Keycloak
        string partId = await GetDisputeAssignToPartIdAsync(ticketNumber, cancellationToken);

        JJDispute dispute = await _oracleDataApi.AcceptJJDisputeAsync(ticketNumber, checkVTC, partId, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
            dispute.TicketNumber, 
            FileHistoryAuditLogEntryType.VSUB, 
            GetUserName(user)); // Dispute approved for resulting by staff

        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTC, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ConcludeJJDisputeAsync(ticketNumber, checkVTC, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTC, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.CancelJJDisputeAsync(ticketNumber, checkVTC, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> ConfirmJJDisputeAsync(string ticketNumber, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ConfirmJJDisputeAsync(ticketNumber, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
            dispute.TicketNumber,
            FileHistoryAuditLogEntryType.JCNF, 
            GetUserName(user)); // Dispute decision confirmed/submitted by JJ
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }

    /// <summary>
    /// Attempts to retrieve a PartId from Keycloak via the JJDispute's jjAssignedTo IDIR field
    /// </summary>
    /// <param name="ticketNumber">JJDispute to retrieve (to reference jjAssignedTo)</param>
    /// <param name="cancellationToken">pass through param</param>
    /// <exception cref="DisputeNotAssignedException">The dispute is not assigned</exception>
    /// <returns></returns>
    internal async Task<string> GetDisputeAssignToPartIdAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        // TCVP-2124
        //  - lookup JJDispute from TCO ORDS
        //  - using jjDispute.jjAssignedTo, lookup partId from keycloakApi
        //  - throw error if either jjAssignedTo or partId is null
        //  - pass partId to _oracleDataApi.AcceptJJDisputeAsync()
        JJDispute jjDispute = await _oracleDataApi.GetJJDisputeAsync(ticketNumber, false, cancellationToken);
        string? assignedTo = jjDispute.JjAssignedTo;

        if (string.IsNullOrEmpty(assignedTo))
        {
            LogNooneIsAssignedToTicket(ticketNumber);
            throw new DisputeNotAssignedException(ticketNumber);
        }

        string? partId = null;
        ICollection<UserRepresentation> userRepresentations = await _keycloakService.UsersByIdirAsync(assignedTo, cancellationToken);
        if (userRepresentations is not null)
        {
            foreach (UserRepresentation userRepresentation in userRepresentations)
            {
                IList<string> attributes = userRepresentation.GetAttributeValues(UserAttributes.PartId);
                if (attributes.Count != 0)
                {
                    if (attributes.Count > 1)
                    {
                        LogUserAssignedToTicketHasManyPartIds(ticketNumber, assignedTo);
                    }

                    // use the first one
                    partId = attributes[0];
                    break;
                }
            }

            if (partId is null)
            {
                // user does not have a part id
                LogUserAssignedToTicketHasNoPartId(ticketNumber, assignedTo);
            }
        }
        else
        {
            LogUserAssignedToTicketNotFound(ticketNumber, assignedTo);
        }

        if (partId is null)
        {
            throw new PartIdNotFoundException(ticketNumber, assignedTo);
        }

        return partId;
    }

    public async Task<string?> GetDisputeAssignToDisplayNameAsync(string assignedTo, CancellationToken cancellationToken)
    {
        string? displayName = null;
        ICollection<UserRepresentation> userRepresentations = await _keycloakService.UsersByIdirAsync(assignedTo, cancellationToken);
        if (userRepresentations is not null)
        {
            foreach (UserRepresentation userRepresentation in userRepresentations)
            {
                IList<string> attributes = userRepresentation.GetAttributeValues(UserAttributes.DisplayName);
                if (attributes.Count != 0)
                {
                    if (attributes.Count > 1)
                    {
                        LogUserAssignedToHasManyDisplayNames(assignedTo);
                    }

                    // use the first one
                    displayName = attributes[0];
                    break;
                }
            }

            if (displayName is null)
            {
                // user does not have a display name
                LogUserAssignedToHasNoDisplayName(assignedTo);
            }
        }
        else
        {
            LogUserAssignedToNotFound(assignedTo);
        }

        return displayName;
    }

    private static string GetUserName(ClaimsPrincipal user)
    {
        return user?.Identity?.Name ?? string.Empty;
    }

    private void AddUnique(List<TrafficCourts.Domain.Models.FileMetadata> target, List<TrafficCourts.Domain.Models.FileMetadata> files)
    {
        HashSet<Guid> existing = new(target.Where(_ => _.FileId.HasValue).Select(_ => _.FileId!.Value));

        foreach (var file in files.Where(_ => _.FileId.HasValue))
        {
            // only add unique files
            if (existing.Add(file.FileId!.Value))
            {
                target.Add(file);
            }
        }
    }

    /// <summary>
    /// Returns the statute description of the given count
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private async Task<string> GetStatuteDescriptionAsync(JJDisputedCount count)
    {
        // Find statute by count's statute ID since the ID is stored in the Description field of the JJDisputedCount
        TrafficCourts.Domain.Models.Statute? statute = await _lookupService.GetByIdAsync(count.Description);
        if (statute is not null)
        {
            string statuteDescription = string.Format("{0} {1} {2}",
                          statute.ActCode, statute.Code, statute.ShortDescriptionText);
            return statuteDescription;
        }
        else
        {
            _logger.LogWarning("Failed to return Statute based on the provided {statuteId}", count.Description);
            return string.Empty;
        }
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, EventName = "UserAssignedToTicketHasNoPartId", Message = "User assigned to ticket has no PartId attribute in Keycloak")]
    private partial void LogUserAssignedToTicketHasNoPartId(
    [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTicketNumber), OmitReferenceName = true)]
        string ticketNumber,
    [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordUsername), OmitReferenceName = true)]
        string assignedToUserId);

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, EventName = "UserAssignedToTicketNotFound", Message = "User assigned to ticket not found in Keycloak")]
    private partial void LogUserAssignedToTicketNotFound(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTicketNumber), OmitReferenceName = true)]
        string ticketNumber,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordUsername), OmitReferenceName = true)]
        string assignedToUserId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, EventName = "UserAssignedToTicketHasManyPartIds", Message = "User assigned to ticket has multiple PartId attributes in Keycloak")]
    private partial void LogUserAssignedToTicketHasManyPartIds(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTicketNumber), OmitReferenceName = true)]
        string ticketNumber,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordUsername), OmitReferenceName = true)]
        string assignedToUserId);

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, EventName = "NooneIsAssignedToTicket", Message = "Noone is assigned to ticket")]
    private partial void LogNooneIsAssignedToTicket(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTicketNumber), OmitReferenceName = true)]
        string ticketNumber);

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, EventName = "UserAssignedToHasManyDisplayNames", Message = "User assigned to ticket has multiple display names in Keycloak")]
    private partial void LogUserAssignedToHasManyDisplayNames(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordUsername), OmitReferenceName = true)]
        string assignedToUserId);

    [LoggerMessage(EventId = 5, Level = LogLevel.Warning, EventName = "UserAssignedToHasNoDisplayName", Message = "User assigned to ticket has no display name attribute in Keycloak")]
    private partial void LogUserAssignedToHasNoDisplayName(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordUsername), OmitReferenceName = true)]
        string assignedToUserId);

    [LoggerMessage(EventId = 6, Level = LogLevel.Debug, EventName = "UserAssignedToNotFound", Message = "User assigned to ticket not found in Keycloak")]
    private partial void LogUserAssignedToNotFound(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordUsername), OmitReferenceName = true)]
        string assignedToUserId);
}

public class PartIdNotFoundException : Exception
{
    public PartIdNotFoundException(string ticketNumber, string assignedTo) : base($"The assigned JJ {assignedTo} on ticket {ticketNumber} does not have a partId available")
    {
        TicketNumber = ticketNumber;
        AssignedTo = assignedTo;
    }
    
    public string TicketNumber { get; init; }
    public string AssignedTo { get; init; }
}

public class DisplayNameNotFoundException : Exception
{
    public DisplayNameNotFoundException(string assignedTo) : base($"The assigned JJ {assignedTo} does not have a displayName available")
    {
        AssignedTo = assignedTo;
    }

    public string AssignedTo { get; init; }
}
