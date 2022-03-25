using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IOracleInterfaceService
    {
        Task<int> CreateDisputeAsync(Dispute disputeToSubmit);
    }
}
