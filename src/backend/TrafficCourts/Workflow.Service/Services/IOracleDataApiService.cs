using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IOracleDataApiService
    {
        Task<long> CreateDisputeAsync(Dispute disputeToSubmit, CancellationToken cancellationToken);
        Task<Dispute?> GetDisputeByNoticeOfDisputeIdAsync(Guid noticeOfDisputeId, CancellationToken cancellationToken);
        Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory, CancellationToken cancellationToken);
        Task<long> CreateFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken);
        Task VerifyDisputeEmailAsync(long disputeId, CancellationToken cancellationToken);
    }
}
