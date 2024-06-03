using TrafficCourts.Arc.Dispute.Service.Models;

namespace TrafficCourts.Arc.Dispute.Service.Services;

public interface IArcFileService
{
    Task CreateArcFileAsync(List<ArcFileRecord> arcFileData, CancellationToken cancellationToken);
}
