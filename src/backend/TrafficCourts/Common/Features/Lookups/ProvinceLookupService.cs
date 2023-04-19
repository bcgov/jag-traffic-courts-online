using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TrafficCourts.Common.Models;
using static System.Collections.Specialized.BitVector32;

namespace TrafficCourts.Common.Features.Lookups;

public class ProvinceLookupService : CachedLookupService<Province>, IProvinceLookupService
{
    public ProvinceLookupService(IConnectionMultiplexer redis, IMemoryCache cache, ILogger<ProvinceLookupService> logger)
        : base("Provinces", redis, cache, TimeSpan.FromHours(1), logger)
    {
    }

    /// <summary>
    /// Returns a specific Province from the Redis Cache based on the provided prov seq no and country id
    /// </summary>
    /// <param name="provSeqNo"></param>
    /// <param name="ctryId"></param>
    /// <returns></returns>
    public async Task<Province?> GetByProvSeqNoCtryIdAsync(string? provSeqNo, string? ctryId)
    {
        var values = await GetListAsync();

        var provinces = values.Where(_ => _.ProvSeqNo == provSeqNo && _.CtryId == ctryId).ToList();
        if (provinces.Count == 0)
        {
            return null;
        }

        if (provinces.Count > 1)
        {
            _logger.LogInformation("{Count} provinces were returned matching {provSeqNo} and {ctryId}, returning first value", provinces.Count, provSeqNo, ctryId);
        }

        return provinces[0];
    }

}
