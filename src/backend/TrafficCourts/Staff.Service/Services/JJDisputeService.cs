using MassTransit;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public class JJDisputeService : IJJDisputeService
{
    private readonly IOracleDataApiClient _oracleDataApi;

    public JJDisputeService(IOracleDataApiClient oracleDataApi)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
    }

    public async Task<ICollection<JJDispute>> GetAllJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetAllJJDisputesAsync(jjAssignedTo, cancellationToken);
    }

    public async Task<JJDispute> GetJJDisputeAsync(string disputeId, bool assignVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.GetJJDisputeAsync(disputeId, assignVTC, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.UpdateJJDisputeAsync(ticketNumber, checkVTC, jjDispute, cancellationToken);

        return dispute;
    }

    public async Task AssignJJDisputesToJJ(List<string> ticketNumbers, string? username, CancellationToken cancellationToken)
    {
        await _oracleDataApi.AssignJJDisputesToJJAsync(ticketNumbers, username, cancellationToken);
    }

    public async Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, string remark, bool checkVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.ReviewJJDisputeAsync(ticketNumber, checkVTC, remark, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await _oracleDataApi.AcceptJJDisputeAsync(ticketNumber, checkVTC, cancellationToken);

        return dispute;
    }
}
