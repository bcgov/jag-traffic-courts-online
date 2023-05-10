using MassTransit;
using System.Security.Claims;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using JJDisputeStatus = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputeStatus;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public class JJDisputeService : IJJDisputeService
{
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly IBus _bus;
    private readonly IStaffDocumentService _documentService;
    private readonly IKeycloakService _keycloakService;
    private readonly IStatuteLookupService _lookupService;
    private readonly ILogger<JJDisputeService> _logger;

    public JJDisputeService(IOracleDataApiClient oracleDataApi, IBus bus, IStaffDocumentService comsService, IKeycloakService keycloakService, IStatuteLookupService lookupService, ILogger<JJDisputeService> logger)
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

    public async Task<JJDispute> GetJJDisputeAsync(long disputeId, string ticketNumber, bool assignVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.GetJJDisputeAsync(ticketNumber, assignVTC, cancellationToken);

        // Search by dispute id
        DocumentProperties properties = new() { TcoDisputeId = disputeId };

        List<FileMetadata> disputeFiles = await _documentService.FindFilesAsync(properties, cancellationToken);

        // search by notice of dispute guid
        if (dispute.NoticeOfDisputeGuid is not null && Guid.TryParse(dispute.NoticeOfDisputeGuid, out Guid noticeOfDisputeId))
        {
            // create new search properties
            properties = new DocumentProperties { NoticeOfDisputeId = noticeOfDisputeId };
            List<FileMetadata> files = await _documentService.FindFilesAsync(properties, cancellationToken);
            AddUnique(disputeFiles, files);
        }

        dispute.FileData = disputeFiles;

        // Populate the statute description of each count of the JJDispute
        foreach (var count in dispute.JjDisputedCounts)
        {
            // Find statute by count's statute ID since the ID is stored in the Description field of the JJDisputedCount
            Statute? statute = await _lookupService.GetByIdAsync(count.Description);
            if (statute != null)
            {
                string statuteDescription = string.Format("{0} {1} {2}",
                              statute.ActCode, statute.Code, statute.ShortDescriptionText);

                // Set the full description back to the JJDisputedCount as part of the JJDispute to be returned
                count.Description = statuteDescription;
            }
            else
            {
                _logger.LogWarning("Failed to return Statute based on the provided {statuteId}", count.Description);
            }
        }

        return dispute;
    }

    public async Task<TicketImageDataJustinDocument> GetJustinDocumentAsync(string ticketNumber, DocumentType documentType, CancellationToken cancellationToken)
    {
        TicketImageDataJustinDocument justinDocument = await _oracleDataApi.GetTicketImageDataAsync(ticketNumber, documentType, cancellationToken);

        return justinDocument;
    }


    public async Task<JJDispute> SubmitAdminResolutionAsync(long disputeId, bool checkVTC, JJDispute jjDispute, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.UpdateJJDisputeAsync(jjDispute.TicketNumber, checkVTC, jjDispute, cancellationToken);

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

    public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, string remark, bool checkVTC, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ReviewJJDisputeAsync(ticketNumber, checkVTC, remark, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistoryWithTicketNumber(
            dispute.TicketNumber,
            FileHistoryAuditLogEntryType.VREV, 
            GetUserName(user)); // Dispute returned to JJ for review
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
        // Get PartId from Keycloak
        string partId = await GetPartIdAsync(ticketNumber, cancellationToken) ?? "";

        if (string.IsNullOrEmpty(partId)) throw new PartIdNotFoundException(ticketNumber);

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

    /// <summary>
    /// Attempts to retrieve a PartId from Keycloak via the JJDispute's jjAssignedTo IDIR field
    /// </summary>
    /// <param name="ticketNumber">JJDispute to retrieve (to reference jjAssignedTo)</param>
    /// <param name="cancellationToken">pass through param</param>
    /// <returns></returns>
    public async Task<string?> GetPartIdAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        // TCVP-2124
        //  - lookup JJDispute from TCO ORDS
        //  - using jjDispute.jjAssignedTo, lookup partId from keycloakApi
        //  - throw error if either jjAssignedTo or partId is null
        //  - pass partId to _oracleDataApi.AcceptJJDisputeAsync()
        JJDispute jjDispute = await _oracleDataApi.GetJJDisputeAsync(ticketNumber, false, cancellationToken);
        string idirUsername = jjDispute.JjAssignedTo ?? "";

        if (!string.IsNullOrEmpty(idirUsername))
        {
            ICollection<UserRepresentation> userRepresentations = await _keycloakService.UsersByIdirAsync(idirUsername, cancellationToken);
            if (userRepresentations is not null)
            {
                foreach (UserRepresentation userRepresentation in userRepresentations)
                {
                    ICollection<string> partIds = _keycloakService.TryGetPartIds(userRepresentation);
                    if (partIds is not null && partIds.Count > 0)
                    {
                        if (partIds.Count > 1)
                        {
                            _logger.LogWarning("idirUsername has more than one partId");
                        }
                        return partIds.First();
                    }
                }
            }
        }

        _logger.LogDebug("Failed to lookup partId for {ticketNumber}", ticketNumber);
        return null;
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

    private static string GetUserName(ClaimsPrincipal user)
    {
        return user?.Identity?.Name ?? string.Empty;
    }
    // ClaimsPrincipal user

    private void AddUnique(List<FileMetadata> target, List<FileMetadata> files)
    {
        foreach (var file in files)
        {
            // only add unique files
            if (!target.Any(_ => _.FileId == file.FileId))
            {
                target.Add(file);
            }
        }
    }
}

[Serializable]
public class PartIdNotFoundException : Exception
{
    public PartIdNotFoundException(string ticketNumber) : base($"Failed to retrieve a partId for the given ticket number: {ticketNumber}")
    {
        TicketNumber = ticketNumber;
    }
    
    public string TicketNumber { get; init; }
}
