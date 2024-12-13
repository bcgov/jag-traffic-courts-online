using TrafficCourts.Domain.Models;

namespace TrafficCourts.Common.Features.Lookups;

public interface IStatuteLookupService
{
    /// <summary>
    /// Returns a specific Statute from the Redis Cache based on the provided section
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    Task<Statute?> GetBySectionAsync(string section, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a specific Statute from the Redis Cache based on the provided ID
    /// </summary>
    /// <param name="statuteId"></param>
    /// <returns></returns>
    Task<Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken);

    Task<IList<Statute>> GetListAsync(CancellationToken cancellationToken);
}
