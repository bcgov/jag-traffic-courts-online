using MassTransit;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.Mappers;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public class JJDisputeService : IJJDisputeService
{
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly IBus _bus;
    private readonly ILogger<JJDisputeService> _logger;

    public JJDisputeService(IOracleDataApiClient oracleDataApi, IBus bus, ILogger<JJDisputeService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<JJDispute>> GetAllJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetJJDisputesAsync(jjAssignedTo, null, null, cancellationToken);
    }

    public async Task<JJDispute> GetJJDisputeAsync(string disputeId, bool assignVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.GetJJDisputeAsync(disputeId, assignVTC, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.UpdateJJDisputeAsync(ticketNumber, checkVTC, jjDispute, cancellationToken);

        if (dispute.Status == JJDisputeStatus.IN_PROGRESS)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, "Dispute decision details saved for later.");
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        } 
        else if (dispute.Status == JJDisputeStatus.CONFIRMED)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, "Dispute decision details confirmed / submitted by JJ.");
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);
        }

        return dispute;
    }

    public async Task AssignJJDisputesToJJ(List<string> ticketNumbers, string? username, CancellationToken cancellationToken)
    {
        await _oracleDataApi.AssignJJDisputesToJJAsync(ticketNumbers, username, cancellationToken);

        // Publish file history
        foreach(string ticketNumber in ticketNumbers)
        {
            SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, "Dispute assigned to JJ.");
            await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken); 
        }
    }

    public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, string remark, bool checkVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ReviewJJDisputeAsync(ticketNumber, checkVTC, remark, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, "Dispute returned to JJ for review.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string remark, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, "JJ requires a court hearing for this dispute.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }


    public async Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.AcceptJJDisputeAsync(ticketNumber, checkVTC, cancellationToken);

        SaveFileHistoryRecord fileHistoryRecord = Mapper.ToFileHistory(ticketNumber, "Dispute approved for resulting by staff.");
        await _bus.PublishWithLog(_logger, fileHistoryRecord, cancellationToken);

        return dispute;
    }
}
