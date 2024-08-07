using TrafficCourts.Domain.Models;

namespace TrafficCourts.Common.Features.Lookups;

public interface IAgencyLookupService : ICachedLookupService<Agency>
{
    /// <summary>
    /// Returns a specific Agency from the Redis Cache based on the agency id
    /// </summary>
    /// <param name="agencyId"></param>
    /// <returns></returns>
    Task<Agency?> GetByIdAsync(string agencyId);
}
