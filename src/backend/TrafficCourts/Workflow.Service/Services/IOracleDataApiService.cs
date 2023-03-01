using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IOracleDataApiService
    {
        Task<long> CreateDisputeAsync(Dispute disputeToSubmit, CancellationToken cancellationToken);
        Task<Dispute?> GetDisputeByNoticeOfDisputeGuidAsync(Guid NoticeOfDisputeGuid, CancellationToken cancellationToken);
        Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory, CancellationToken cancellationToken);
        Task<long> CreateFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken);
        Task VerifyDisputeEmailAsync(long disputeId, CancellationToken cancellationToken);
        Task<Dispute> ResetDisputeEmailAsync(long disputeId, string emailAddress, CancellationToken cancellationToken);
        Task<ICollection<DisputeResult>> SearchDisputeAsync(string? ticketNumber, string? issuedTime, string? noticeOfDisputeGuid, CancellationToken cancellationToken);
        Task<Dispute> GetDisputeByIdAsync(long disputeId, bool isAssign, CancellationToken cancellationToken);
        Task<Dispute> UpdateDisputeAsync(long disputeId, Dispute dispute, CancellationToken cancellationToken);
        Task<ICollection<JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber, System.Threading.CancellationToken cancellationToken);
        Task<JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, CancellationToken cancellationToken);

        // DisputeUpdateRequest endpoints
        Task<ICollection<DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long disputeId, Status? disputeUpdateRequestStatus, CancellationToken cancellationToken);
        Task<long> SaveDisputeUpdateRequestAsync(string guid, DisputeUpdateRequest disputeUpdateRequest, CancellationToken cancellationToken);
        Task<DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long disputeUpdateRequestId, DisputeUpdateRequestStatus disputeUpdateRequestStatus, CancellationToken cancellationToken);
    }
}
