using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for Class1
/// </summary>
public class DisputeService : IDisputeService
{
    private readonly string _oracleDataApiBaseUrl;
    private readonly ILogger<DisputeService> _logger;

    public DisputeService(String oracleDataApiBaseUrl, ILogger<DisputeService> logger)
    {
        ArgumentNullException.ThrowIfNull(oracleDataApiBaseUrl);
        ArgumentNullException.ThrowIfNull(logger);
        _oracleDataApiBaseUrl = oracleDataApiBaseUrl;
        _logger = logger;
    }

    /// <summary>
    /// Returns a new inialized instance of the OracleDataApi_v1_0Client
    /// </summary>
    /// <returns></returns>
    private OracleDataApi_v1_0Client GetOracleDataApi()
    {
        OracleDataApi_v1_0Client client = new(new HttpClient());
        client.BaseUrl = _oracleDataApiBaseUrl;
        return client;
    }

    public async Task<ICollection<Dispute>> GetAllDisputesAsync(CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().GetAllDisputesAsync(cancellationToken);
    }

    public async Task<int> SaveDisputeAsync(Dispute dispute, CancellationToken cancellationToken)
    {
        // TODO: trigger submit Dispute to ARC, send email to client
        return await GetOracleDataApi().SaveDisputeAsync(dispute, cancellationToken);
    }

    public async Task<Dispute> GetDisputeAsync(int disputeId, CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().GetDisputeAsync(disputeId, cancellationToken);
    }

    public async Task<Dispute> UpdateDisputeAsync(int disputeId, Dispute dispute, System.Threading.CancellationToken cancellationToken)
    {
        return await GetOracleDataApi().UpdateDisputeAsync(disputeId, dispute, cancellationToken);
    }

    public async Task CancelDisputeAsync(int disputeId, CancellationToken cancellationToken)
    {
        await GetOracleDataApi().CancelDisputeAsync(disputeId, cancellationToken);
    }

    public async Task RejectDisputeAsync(int disputeId, string rejectedReason, CancellationToken cancellationToken)
    {
        await GetOracleDataApi().RejectDisputeAsync(disputeId, rejectedReason, cancellationToken);
    }

    public async Task SubmitDisputeAsync(int disputeId, CancellationToken cancellationToken)
    {
        await GetOracleDataApi().SubmitDisputeAsync(disputeId, cancellationToken);
    }

    public async Task DeleteDisputeAsync(int disputeId, CancellationToken cancellationToken)
    {
        await GetOracleDataApi().DeleteDisputeAsync(disputeId, cancellationToken);
    }
}
