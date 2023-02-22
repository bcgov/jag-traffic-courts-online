using MassTransit;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;
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
    private readonly ILogger<JJDisputeService> _logger;

    public JJDisputeService(IOracleDataApiClient oracleDataApi, IBus bus, IStaffDocumentService comsService, IKeycloakService keycloakService, ILogger<JJDisputeService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _documentService = comsService ?? throw new ArgumentNullException(nameof(comsService));
        _keycloakService = keycloakService ?? throw new ArgumentNullException(nameof(keycloakService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<JJDispute>> GetAllJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetJJDisputesAsync(jjAssignedTo, null, cancellationToken);
    }

    public async Task<JJDispute> GetJJDisputeAsync(string disputeId, bool assignVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.GetJJDisputeAsync(disputeId, assignVTC, cancellationToken);

        Dictionary<string, string> documentSearchParam = new();
        documentSearchParam.Add("ticket-number", disputeId);
        if (dispute.NoticeOfDisputeGuid is not null) documentSearchParam.Add("notice-of-dispute-id", dispute.NoticeOfDisputeGuid);
        documentSearchParam.Add("dispute-id", dispute.Id.ToString());

        dispute.FileData = await _documentService.GetFilesBySearchAsync(documentSearchParam, null, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.UpdateJJDisputeAsync(ticketNumber, checkVTC, jjDispute, cancellationToken);

        if (dispute.Status == JJDisputeStatus.IN_PROGRESS)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                jjDispute.OccamDisputeId,
                FileHistoryAuditLogEntryType.JPRG); // Dispute decision details saved for later
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }
        else if (dispute.Status == JJDisputeStatus.CONFIRMED)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                jjDispute.OccamDisputeId,
                FileHistoryAuditLogEntryType.JCNF); // Dispute decision confirmed/submitted by JJ
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }

        return dispute;
    }

    public async Task AssignJJDisputesToJJ(List<string> ticketNumbers, string? username, CancellationToken cancellationToken)
    {
        await _oracleDataApi.AssignJJDisputesToJJAsync(ticketNumbers, username, cancellationToken);

        // Publish file history
        foreach (string ticketNumber in ticketNumbers)
        {
            JJDispute dispute = await _oracleDataApi.GetJJDisputeAsync(ticketNumber, false, cancellationToken);

            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                dispute.OccamDisputeId,
                FileHistoryAuditLogEntryType.JASG); // Dispute assigned to JJ
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }
    }

    public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, string remark, bool checkVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ReviewJJDisputeAsync(ticketNumber, checkVTC, remark, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
            dispute.OccamDisputeId,
            FileHistoryAuditLogEntryType.VREV); // Dispute returned to JJ for review
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string? remark, CancellationToken cancellationToken)
    {
        try
        {
            JJDispute dispute = await _oracleDataApi.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, cancellationToken);

            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                dispute.OccamDisputeId,
                FileHistoryAuditLogEntryType.JDIV); // Dispute change of plea required / Divert to court appearance
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

            return dispute;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "could not set status to require court hearing");
            throw;
        }

    }

    public async Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTC, CancellationToken cancellationToken)
    {
        // Get PartId from Keycloak
        string partId = await GetPartIdAsync(ticketNumber, cancellationToken);

        JJDispute dispute = await _oracleDataApi.AcceptJJDisputeAsync(ticketNumber, checkVTC, partId, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
            dispute.OccamDisputeId,
            FileHistoryAuditLogEntryType.VSUB); // Dispute approved for resulting by staff
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }

    /// <summary>
    /// Attempts to retrieve a PartId from Keycloak via the JJDispute's jjAssignedTo IDIR field
    /// </summary>
    /// <param name="ticketNumber">JJDispute to retrieve (to reference jjAssignedTo)</param>
    /// <param name="cancellationToken">pass through param</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If jjAssignedTo or the PartId in Keycloak is null</exception>
    public async Task<string> GetPartIdAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        // TCVP-2124
        //  - lookup JJDispute from TCO ORDS
        //  - using jjDispute.jjAssignedTo, lookup partId from keycloakApi
        //  - throw error if either jjAssignedTo or partId is null
        //  - pass partId to _oracleDataApi.AcceptJJDisputeAsync()
        JJDispute jjDispute = await _oracleDataApi.GetJJDisputeAsync(ticketNumber, false, cancellationToken);
        string idirUsername = jjDispute.JjAssignedTo ?? throw new ArgumentNullException("JJDispute is not assigned. Failed to lookup partId.");
        if (!JJDisputeHearingType.WRITTEN_REASONS.Equals(jjDispute.HearingType))
        {
            throw new ArgumentException("JJDispute's HearingType is not WRITTEN_REASONS.");
        }

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

        throw new ArgumentNullException("Failed to lookup partId.");
    }

    public async Task<JJDispute> ConfirmJJDisputeAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ConfirmJJDisputeAsync(ticketNumber, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
            dispute.OccamDisputeId,
            FileHistoryAuditLogEntryType.JCNF); // Dispute decision confirmed/submitted by JJ
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }
}
