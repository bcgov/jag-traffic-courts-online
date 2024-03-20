using TrafficCourts.Domain.Models;

namespace TrafficCourts.Interfaces;

public interface IOracleDataApiService
{
    #region IOracleDataApiClient methods
    Task<JJDispute> AcceptJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, string partId, CancellationToken cancellationToken);
    Task AssignJJDisputesToJJAsync(IEnumerable<string> ticketNumbers, string? jjUsername, CancellationToken cancellationToken);
    Task<Dispute> CancelDisputeAsync(long id, string body, CancellationToken cancellationToken);
    Task<JJDispute> CancelJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, CancellationToken cancellationToken);
    Task CodeTableRefreshAsync(CancellationToken cancellationToken);
    Task<JJDispute> ConcludeJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, CancellationToken cancellationToken);
    Task<JJDispute> ConfirmJJDisputeAsync(string ticketNumber, CancellationToken cancellationToken);
    Task DeleteDisputeAsync(long id, CancellationToken cancellationToken);
    Task DeleteJJDisputeAsync(long? jjDisputeId, string ticketNumber, CancellationToken cancellationToken);
    Task<ICollection<DisputeResult>> FindDisputeStatusesAsync(string ticketNumber, string? issuedTime, string? noticeOfDisputeGuid, ExcludeStatus2? excludeStatus, CancellationToken cancellationToken);
    Task<ICollection<DisputeListItem>> GetAllDisputesAsync(DateTimeOffset? newerThan, ExcludeStatus? excludeStatus, CancellationToken cancellationToken);
    Task<Dispute> GetDisputeAsync(long id, bool isAssign, CancellationToken cancellationToken);
    Task<Dispute> GetDisputeByNoticeOfDisputeGuidAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<DisputeUpdateRequest>> GetDisputeUpdateRequestsAsync(long? id, Status? status, CancellationToken cancellationToken);
    Task<ICollection<EmailHistory>> GetEmailHistoryByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken);
    Task<ICollection<FileHistory>> GetFileHistoryByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken);
    Task<JJDispute> GetJJDisputeAsync(string ticketNumber, bool assignVTC, CancellationToken cancellationToken);
    Task<ICollection<JJDispute>> GetJJDisputesAsync(string? jjAssignedTo, string? ticketNumber, CancellationToken cancellationToken);
    Task<TicketImageDataJustinDocument> GetTicketImageDataAsync(string ticketNumber, DocumentType documentType, CancellationToken cancellationToken);
    Task<long> InsertEmailHistoryAsync(EmailHistory body, CancellationToken cancellationToken);
    Task<long> InsertFileHistoryAsync(FileHistory body, CancellationToken cancellationToken);
    Task<Dispute> RejectDisputeAsync(long id, string body, CancellationToken cancellationToken);
    Task<JJDispute> RequireCourtHearingJJDisputeAsync(string ticketNumber, string? remark, CancellationToken cancellationToken);
    Task<Dispute> ResetDisputeEmailAsync(long id, string email, CancellationToken cancellationToken);
    Task<JJDispute> ReviewJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, bool recalled, string body, CancellationToken cancellationToken);
    Task<long> SaveDisputeAsync(Dispute body, CancellationToken cancellationToken);
    Task<long> SaveDisputeUpdateRequestAsync(string guid, DisputeUpdateRequest body, CancellationToken cancellationToken);
    Task<Dispute> SubmitDisputeAsync(long id, CancellationToken cancellationToken);
    Task UnassignDisputesAsync(CancellationToken cancellationToken);
    Task<Dispute> UpdateDisputeAsync(long id, Dispute body, CancellationToken cancellationToken);
    Task<DisputeUpdateRequest> UpdateDisputeUpdateRequestStatusAsync(long id, DisputeUpdateRequestStatus disputeUpdateRequestStatus, CancellationToken cancellationToken);
    Task<JJDispute> UpdateJJDisputeAsync(string ticketNumber, bool checkVTCAssigned, JJDispute body, CancellationToken cancellationToken);
    Task<JJDispute> UpdateJJDisputeCascadeAsync(string ticketNumber, bool checkVTCAssigned, JJDispute body, CancellationToken cancellationToken);
    Task<Dispute> ValidateDisputeAsync(long id, CancellationToken cancellationToken);
    Task VerifyDisputeEmailAsync(long id, CancellationToken cancellationToken);
    #endregion

    #region Non Oracle Data API signatures
    Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory, CancellationToken cancellationToken);
    Task<long> CreateFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken);
    Task<Dispute> GetDisputeByIdAsync(long disputeId, bool isAssign, CancellationToken cancellationToken);
    Task<Dispute?> GetDisputeByNoticeOfDisputeGuidAsync(Guid noticeOfDisputeGuid, CancellationToken cancellationToken);
    Task<IList<DisputeResult>> SearchDisputeAsync(string? ticketNumber, string? issuedTime, Guid? noticeOfDisputeGuid, ExcludeStatus2? excludeStatus, CancellationToken cancellationToken);
    #endregion
}
