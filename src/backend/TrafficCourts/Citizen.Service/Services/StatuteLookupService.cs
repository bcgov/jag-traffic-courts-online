using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.OrdsDataService.Justin;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Citizen.Service.Services
{
    public class StatuteLookupService : StatuteLookupServiceBase<StatuteLookupService>
    {
        public StatuteLookupService(IStatuteRepository repository, IFusionCache cache, ILogger<StatuteLookupService> logger)
            : base(repository, cache, logger)
        {
        }
        protected override string GetCacheKey() => Caching.Cache.Api.Statutes(2);
    }
}
