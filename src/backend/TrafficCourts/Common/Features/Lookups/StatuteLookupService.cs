using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Linq;
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
            if (string.IsNullOrEmpty(section))
            {
                return null;
            }

            var sections = await GetStatutesAsync(_ => _.Code == section);
            if (sections.Count == 0)
            {
                return null;
            }

            if (sections.Count > 1)
            {
                _logger.LogInformation("{Count} sections were returned matching {Section}, returning first value", sections.Count, section);
            }

            return sections[0];
        }

        public async Task<Statute?> GetByIdAsync(string statuteId)
        {
            if (string.IsNullOrEmpty(statuteId))
            {
                return null;
            }

            var sections = await GetStatutesAsync(statute => statute.Id == statuteId);
            if (sections.Count == 0)
            {
                return null;
            }

            if (sections.Count > 1)
            {
                _logger.LogInformation("{Count} sections were returned matching {Id}, returning first value", sections.Count, statuteId);
            }

            return sections[0];
        }

        private async Task<List<Statute>> GetStatutesAsync(Func<Statute, bool> predicate)
        {
            var values = await GetListAsync();
            var sections = values.Where(predicate).ToList();
            return sections;
        }
    }
}
