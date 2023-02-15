using MassTransit;
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
    private readonly ILogger<JJDisputeService> _logger;

    public JJDisputeService(IOracleDataApiClient oracleDataApi, IBus bus, IStaffDocumentService comsService, ILogger<JJDisputeService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _documentService = comsService ?? throw new ArgumentNullException(nameof(comsService));
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

        // TODO: Add search parameter "notice-of-dispute-id" for returning other documents for the associated dispute that were uploaded by the citizen
        // when there will be an endpoint to return disputes by ticket number.

        //dispute.FileData = await _documentService.GetFilesBySearchAsync(documentSearchParam, null, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.UpdateJJDisputeAsync(ticketNumber, checkVTC, jjDispute, cancellationToken);

        if (jjDispute.OccamDisputeId != null)
        {
            if (dispute.Status == JJDisputeStatus.IN_PROGRESS)
            {
                SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                    jjDispute.OccamDisputeId.Value,
                    FileHistoryAuditLogEntryType.JPRG); // Dispute decision details saved for later
                await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
            }
            else if (dispute.Status == JJDisputeStatus.CONFIRMED)
            {
                SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                    jjDispute.OccamDisputeId.Value,
                    FileHistoryAuditLogEntryType.JCNF); // Dispute decision confirmed/submitted by JJ
                await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
            }
        }
        else
        {
            _logger.LogDebug("File history record could not be saved due to missing OccamDisputeId of JJDispute with {JJDisputeId}", jjDispute.Id);
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

            if (dispute.OccamDisputeId != null)
            {
                SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                    dispute.OccamDisputeId.Value,
                    FileHistoryAuditLogEntryType.JASG); // Dispute assigned to JJ
                await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
            }
            else
            {
                _logger.LogDebug("File history record could not be saved due to missing OccamDisputeId of JJDispute with {JJDisputeId}", dispute.Id);
            }
        }
    }

    public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, string remark, bool checkVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ReviewJJDisputeAsync(ticketNumber, checkVTC, remark, cancellationToken);
        if (dispute.OccamDisputeId != null)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                dispute.OccamDisputeId.Value,
                FileHistoryAuditLogEntryType.VREV); // Dispute returned to JJ for review
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }
        else
        {
            _logger.LogDebug("File history record could not be saved due to missing OccamDisputeId of JJDispute with {JJDisputeId}", dispute.Id);
        }

        return dispute;
    }

    public async Task<JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string? remark, CancellationToken cancellationToken)
    {
        try
        {
            JJDispute dispute = await _oracleDataApi.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, cancellationToken);

            if (dispute.OccamDisputeId != null)
            {
                SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                    dispute.OccamDisputeId.Value, 
                    FileHistoryAuditLogEntryType.JDIV); // Dispute change of plea required / Divert to court appearance
                await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
            }
            else
            {
                _logger.LogDebug("File history record could not be saved due to missing OccamDisputeId of JJDispute with {JJDisputeId}", dispute.Id);
            }

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
        JJDispute dispute = await _oracleDataApi.AcceptJJDisputeAsync(ticketNumber, checkVTC, null, null, cancellationToken);

        if (dispute.OccamDisputeId != null)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                dispute.OccamDisputeId.Value,
                FileHistoryAuditLogEntryType.VSUB); // Dispute approved for resulting by staff
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }
        else
        {
            _logger.LogDebug("File history record could not be saved due to missing OccamDisputeId of JJDispute with {JJDisputeId}", dispute.Id);
        }

        return dispute;
    }

    public async Task<JJDispute> ConfirmJJDisputeAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ConfirmJJDisputeAsync(ticketNumber, cancellationToken);

        if (dispute.OccamDisputeId != null)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(
                dispute.OccamDisputeId.Value, 
                FileHistoryAuditLogEntryType.JCNF); // Dispute decision confirmed/submitted by JJ
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }
        else
        {
            _logger.LogDebug("File history record could not be saved due to missing OccamDisputeId of JJDispute with {JJDisputeId}", dispute.Id);
        }

        return dispute;
    }
}
