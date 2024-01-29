using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public interface ICountryLookupService : ICachedLookupService<Country>
{
    /// <summary>
    /// Returns a specific Country from the Redis Cache based on the country id
    /// </summary>
    /// <param name="ctryId"></param>
    /// <returns></returns>
    Task<Country?> GetByIdAsync(string ctryId);

}
