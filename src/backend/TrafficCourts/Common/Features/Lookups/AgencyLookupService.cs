using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Common.Features.Lookups;

public class AgencyLookupService : CachedLookupService<Agency>, IAgencyLookupService
{
    public AgencyLookupService(IConnectionMultiplexer redis, IMemoryCache cache, ILogger<AgencyLookupService> logger)
        : base("Agencies", redis, cache, TimeSpan.FromHours(1), logger)
    {
    }

    public async Task<Agency?> GetByIdAsync(string agencyId)
    {
        if (string.IsNullOrEmpty(agencyId))
        {
            return null;
        }

        var agencies = await GetAgenciesAsync(agency => agency.Id == agencyId);
        if (agencies.Count == 0)
        {
            return null;
        }

        if (agencies.Count > 1)
        {
            _logger.LogInformation("{Count} agencies were returned matching {Id}, returning first value", agencies.Count, agencyId);
        }

        return agencies[0];
    }

    private async Task<List<Agency>> GetAgenciesAsync(Func<Agency, bool> predicate)
    {
        var values = await GetListAsync();
        var sections = values.Where(predicate).ToList();
        return sections;
    }
}
