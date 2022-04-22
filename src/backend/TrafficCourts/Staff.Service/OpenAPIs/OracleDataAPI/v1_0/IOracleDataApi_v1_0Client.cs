namespace TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;

public interface IOracleDataApi_v1_0Client
{
    string BaseUrl { get; set; }

    Task CancelDisputeAsync(int id, CancellationToken cancellationToken);
    Task DeleteDisputeAsync(int id, CancellationToken cancellationToken);
    Task<ICollection<Dispute>> GetAllDisputesAsync(CancellationToken cancellationToken);
    Task<Dispute> GetDisputeAsync(int id, CancellationToken cancellationToken);
    Task RejectDisputeAsync(int id, string body, CancellationToken cancellationToken);
    Task<int> SaveDisputeAsync(Dispute body, CancellationToken cancellationToken);
    Task SubmitDisputeAsync(int id, CancellationToken cancellationToken);
    Task<Dispute> UpdateDisputeAsync(int id, Dispute body, CancellationToken cancellationToken);
}
