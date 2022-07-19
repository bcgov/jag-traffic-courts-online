using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IOracleDataApiService
    {
        Task<long> CreateDisputeAsync(Dispute disputeToSubmit);
    }
}
