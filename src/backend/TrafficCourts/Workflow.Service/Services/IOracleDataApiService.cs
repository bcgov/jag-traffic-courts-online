using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IOracleDataApiService
    {
        Task<Guid> CreateDisputeAsync(NoticeOfDispute disputeToSubmit);
    }
}
