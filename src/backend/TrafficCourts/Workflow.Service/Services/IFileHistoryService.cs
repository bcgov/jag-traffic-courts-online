using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Services
{
    public interface IFileHistoryService
    {
        Task<long> SaveFileHistoryAsync(FileHistoryRecord fileHistoryRecord, CancellationToken cancellationToken);
    }
}
