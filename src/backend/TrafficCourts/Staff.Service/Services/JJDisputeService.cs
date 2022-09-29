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

    [Obsolete("Use field _oracleDataApi directly")]
    private IOracleDataApiClient GetOracleDataApi() => _oracleDataApi;

    public async Task<ICollection<JJDispute>> GetAllJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().GetAllJJDisputesAsync(jjAssignedTo, cancellationToken);
    }

    public async Task<JJDispute> GetJJDisputeAsync(string disputeId, bool assignVTC, CancellationToken cancellationToken)
    {
        JJDispute dispute = await GetOracleDataApi().GetJJDisputeAsync(disputeId, assignVTC, cancellationToken);

        return dispute;
    }

    public async Task<JJDispute> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken)
    {
        JJDispute dispute = await GetOracleDataApi().UpdateJJDisputeAsync(ticketNumber, checkVTC, jjDispute, cancellationToken);

        return dispute;
    }
}
