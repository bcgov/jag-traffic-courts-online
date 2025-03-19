namespace TrafficCourts.Citizen.Service.Services.Lookups;

public interface IStatuteLookupService
{
    /// <summary>
    /// Returns a specific Statute from the Redis Cache based on the provided section
    /// </summary>
    /// <param name="section"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TrafficCourts.Domain.Models.Statute?> GetBySectionAsync(string section, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a specific Statute from the Redis Cache based on the provided ID
    /// </summary>
    /// <param name="statuteId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TrafficCourts.Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken);

    Task<IList<TrafficCourts.Domain.Models.Statute>> GetListAsync(CancellationToken cancellationToken);
}
