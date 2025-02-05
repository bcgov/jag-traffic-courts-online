namespace TrafficCourts.Staff.Service.Services;

public interface IStatuteLookupService
{
    Task<Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken);
}
