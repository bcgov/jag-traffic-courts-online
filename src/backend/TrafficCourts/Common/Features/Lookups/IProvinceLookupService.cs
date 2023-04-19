using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public interface IProvinceLookupService : ICachedLookupService<Province>
{
    /// <summary>
    /// Returns a specific Province from the Redis Cache based on the provided prov seq no and country id
    /// </summary>
    /// <param name="provSeqNo"></param>
    /// <param name="ctryId"></param>
    /// <returns></returns>
    Task<Province?> GetByProvSeqNoCtryIdAsync(string? provSeqNo, string? ctryId);

}
