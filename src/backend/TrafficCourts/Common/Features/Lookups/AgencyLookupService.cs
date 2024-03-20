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
}
