using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public class LanguageLookupService : CachedLookupService<Language>, ILanguageLookupService
{
    public LanguageLookupService(IConnectionMultiplexer redis, IMemoryCache cache, ILogger<LanguageLookupService> logger)
        : base("Languages", redis, cache, TimeSpan.FromHours(1), logger)
    {
    }
}
