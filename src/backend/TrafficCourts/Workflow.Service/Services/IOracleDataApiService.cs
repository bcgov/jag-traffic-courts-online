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
        Task<Dispute> GetDisputeByIdAsync(long disputeId, CancellationToken cancellationToken);
        Task<Dispute> UpdateDisputeAsync(long disputeId, Dispute dispute, CancellationToken cancellationToken);
        Task<ICollection<JJDispute>> GetJJDisputesAsync(string jjAssignedTo, string ticketNumber, System.Threading.CancellationToken cancellationToken);

        // DisputantUpdateRequest endpoints
        Task<ICollection<DisputantUpdateRequest>> GetDisputantUpdateRequestsAsync(long disputeId, Status? disputantUpdateRequestStatus, CancellationToken cancellationToken);
        Task<long> SaveDisputantUpdateRequestAsync(string guid, DisputantUpdateRequest disputantUpdateRequest, CancellationToken cancellationToken);
        Task<DisputantUpdateRequest> UpdateDisputantUpdateRequestStatusAsync(long disputantUpdateRequestId, DisputantUpdateRequestStatus disputantUpdateRequestStatus, CancellationToken cancellationToken);
    }
}
