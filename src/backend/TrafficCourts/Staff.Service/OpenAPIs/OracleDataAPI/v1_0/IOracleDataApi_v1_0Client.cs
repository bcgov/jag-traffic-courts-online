namespace TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;

/// <summary>
/// Generated/Extracted from OracleDataApi_v1_0Client, partial extraction (only public methods with CancellationToken as a param)
/// </summary>
public interface IOracleDataApi_v1_0Client
{
    Task CodeTableRefreshAsync(CancellationToken cancellationToken);
    Task DeleteDisputeAsync(int disputeId, CancellationToken cancellationToken);
    Task<ICollection<Dispute>> GetAllDisputesAsync(CancellationToken cancellationToken);
    Task<Dispute> GetDisputeAsync(int disputeId, CancellationToken cancellationToken);
    Task<int> SaveDisputeAsync(Dispute body, CancellationToken cancellationToken);
    Task<Dispute> UpdateAsync(Dispute body, CancellationToken cancellationToken);
}
