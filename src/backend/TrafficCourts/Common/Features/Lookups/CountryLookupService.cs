using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public class CountryLookupService : CachedLookupService<Country>, ICountryLookupService
{
        public CountryLookupService(IConnectionMultiplexer redis, IMemoryCache cache, ILogger<CountryLookupService> logger)
            : base("Countries", redis, cache, TimeSpan.FromHours(1), logger)
        {
        }

        public async Task<Country?> GetByIdAsync(string ctryId)
        {
            if (string.IsNullOrEmpty(ctryId))
            {
                return null;
            }

            var countries = await GetCountriesAsync(statute => statute.CtryId == ctryId);
            if (countries.Count == 0)
            {
                return null;
            }

            if (countries.Count > 1)
            {
                _logger.LogInformation("{Count} countries were returned matching {Id}, returning first value", countries.Count, ctryId);
            }

            return countries[0];
        }

        private async Task<List<Country>> GetCountriesAsync(Func<Country, bool> predicate)
        {
            var values = await GetListAsync();
            var sections = values.Where(predicate).ToList();
            return sections;
        }

}
