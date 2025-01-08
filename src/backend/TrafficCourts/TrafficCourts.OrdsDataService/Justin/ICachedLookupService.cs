using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.OrdsDataService.Justin;

public interface ICachedLookupService<T>
{
    Task<List<T>> GetListAsync(string cacheKey, TimeSpan cacheDuration, CancellationToken cancellationToken);
}

internal abstract class CachedLookupService<T> : ICachedLookupService<T>
{
    private readonly IFusionCache _cache;

    public CachedLookupService(IFusionCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<List<T>> GetListAsync(string cacheKey, TimeSpan cacheDuration, CancellationToken cancellationToken)
    {
        // cache the data directly fetched from ORDS

        // enable fail-safe caching
        // - cache the data for the duration the caller requested
        // - use the cached data up to one day after the requested duration even if it is expired
        // - use the expired data for one hour before trying again

        var items = await _cache.GetOrSetAsync<List<T>>(
            cacheKey,
            GetItemsAsync,
            options => options
                .SetDuration(cacheDuration)
                .SetFailSafe(true, TimeSpan.FromDays(1), TimeSpan.FromHours(1)),
            cancellationToken);

        return items;
    }

    protected abstract Task<List<T>> GetItemsAsync(CancellationToken cancellationToken);
}

internal class AgencyLookupService : CachedLookupService<Agency>
{
    private readonly IAgencyRepository _repository;

    public AgencyLookupService(IAgencyRepository repository, IFusionCache cache) : base(cache)
    {
        _repository = repository;
    }

    protected override Task<List<Agency>> GetItemsAsync(CancellationToken cancellationToken)
        => _repository.GetListAsync(cancellationToken);
}

internal class CityLookupService : CachedLookupService<City>
{
    private readonly ICityRepository _repository;

    public CityLookupService(ICityRepository repository, IFusionCache cache) : base(cache)
    {
        _repository = repository;
    }

    protected override Task<List<City>> GetItemsAsync(CancellationToken cancellationToken)
        => _repository.GetListAsync(cancellationToken);
}

internal class CountryLookupService : CachedLookupService<Country>
{
    private readonly ICountryRepository _repository;

    public CountryLookupService(ICountryRepository repository, IFusionCache cache) : base(cache)
    {
        _repository = repository;
    }

    protected override Task<List<Country>> GetItemsAsync(CancellationToken cancellationToken)
        => _repository.GetListAsync(cancellationToken);
}

internal class LanguageLookupService : CachedLookupService<Language>
{
    private readonly ILanguageRepository _repository;

    public LanguageLookupService(ILanguageRepository repository, IFusionCache cache) : base(cache)
    {
        _repository = repository;
    }

    protected override Task<List<Language>> GetItemsAsync(CancellationToken cancellationToken)
        => _repository.GetListAsync(cancellationToken);
}


internal class ProvinceLookupService : CachedLookupService<Province>
{
    private readonly IProvinceRepository _repository;

    public ProvinceLookupService(IProvinceRepository repository, IFusionCache cache) : base(cache)
    {
        _repository = repository;
    }

    protected override Task<List<Province>> GetItemsAsync(CancellationToken cancellationToken)
        => _repository.GetListAsync(cancellationToken);
}


internal class StatuteLookupService : CachedLookupService<Statute>
{
    private readonly IStatuteRepository _repository;

    public StatuteLookupService(IStatuteRepository repository, IFusionCache cache) : base(cache)
    {
        _repository = repository;
    }

    protected override Task<List<Statute>> GetItemsAsync(CancellationToken cancellationToken)
        => _repository.GetListAsync(cancellationToken);
}
