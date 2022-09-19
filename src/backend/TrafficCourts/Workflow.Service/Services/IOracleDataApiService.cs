using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IOracleDataApiService
    {
        Task<long> CreateDisputeAsync(Dispute disputeToSubmit);
        Task<Dispute> GetDisputeByEmailVerificationTokenAsync(string emailVerificationToken);
        Task ValidateDisputeEmailAsync(string emailVerificationToken);
        Task<long> CreateEmailHistoryAsync(EmailHistory emailHistory);
    }
}
