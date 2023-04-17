using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public class ProvinceLookupService : CachedLookupService<Province>, IProvinceLookupService
{
    public ProvinceLookupService(IConnectionMultiplexer redis, IMemoryCache cache, ILogger<ProvinceLookupService> logger)
        : base("Provinces", redis, cache, TimeSpan.FromHours(1), logger)
    {
    }
}
