namespace TrafficCourts.Staff.Service.Services;

public interface IAgencyLookupService
{
    Task<List<TrafficCourts.Domain.Models.Agency>> GetListAsync(CancellationToken cancellationToken);

    Task<TrafficCourts.Domain.Models.Agency?> GetByIdAsync(string agencyId, CancellationToken cancellationToken);
}
