using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups
{
    public class StatuteLookupService : CachedLookupService<Statute>, IStatuteLookupService
    {
        public StatuteLookupService(IConnectionMultiplexer redis, IMemoryCache cache, ILogger<StatuteLookupService> logger)
            : base("Statutes", redis, cache, TimeSpan.FromHours(1), logger)
        {
        }

        public async Task<Statute?> GetBySectionAsync(string section)
        {
            var values = await GetListAsync();

            var sections = values.Where(_ => _.Code == section).ToList();
            if (sections.Count == 0)
            {
                return null;
            }

            if (sections.Count >= 1)
            {
                _logger.LogInformation("{Count} sections were returned matching {Section}, returning first value", sections.Count, section);
            }

            return sections[0];
        }
    }
}
