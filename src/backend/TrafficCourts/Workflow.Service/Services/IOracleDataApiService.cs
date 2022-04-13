using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IOracleDataApiService
    {
        Task<int> CreateDisputeAsync(Dispute disputeToSubmit);
    }
}
