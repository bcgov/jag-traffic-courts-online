using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public interface IStatuteLookupService : ICachedLookupService<Statute>
{
    /// <summary>
    /// Returns a specific Statute from the Redis Cache based on the provided section
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    Task<Statute?> GetBySectionAsync(string section);

    /// <summary>
    /// Returns a specific Statute from the Redis Cache based on the provided ID
    /// </summary>
    /// <param name="statuteId"></param>
    /// <returns></returns>
    Task<Statute?> GetByIdAsync(string statuteId);
}
