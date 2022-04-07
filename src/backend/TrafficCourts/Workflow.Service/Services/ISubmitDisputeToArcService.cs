using TrafficCourts.Workflow.Service.Models;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface ISubmitDisputeToArcService
    {
        Task SubmitDisputeToArcAsync(TcoDisputeTicket approvedDispute);
    }
}
