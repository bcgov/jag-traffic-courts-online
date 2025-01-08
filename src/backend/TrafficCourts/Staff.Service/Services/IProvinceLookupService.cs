using System.Text;
using System.Threading;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Services;

public interface IAgencyLookupService
{
    Task<List<TrafficCourts.Domain.Models.Agency>> GetListAsync(CancellationToken cancellationToken);

    Task<TrafficCourts.Domain.Models.Agency?> GetByIdAsync(string agencyId, CancellationToken cancellationToken);
}

public class AgencyLookupService : IAgencyLookupService
{
    private readonly ICachedLookupService<Agency> _repository;

    public AgencyLookupService(ICachedLookupService<Agency> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Domain.Models.Agency?> GetByIdAsync(string agencyId, CancellationToken cancellationToken)
    {
        if (decimal.TryParse(agencyId, out decimal id))
        {
            var items = await GetAsync(cancellationToken);
            return items.FirstOrDefault(x => x.agen_id == id)?.ToDomainModel();
        }

        return null;
    }

    public async Task<List<Domain.Models.Agency>> GetListAsync(CancellationToken cancellationToken)
    {
        var items = await GetAsync(cancellationToken);
        var models = items.Select(x => x.ToDomainModel()).ToList();
        return models;
    }

    private async Task<List<Agency>> GetAsync(CancellationToken cancellationToken)
    {
        string key = Caching.Cache.Api.Agencies(2);
        var items = await _repository.GetListAsync(key, TimeSpan.FromHours(2), cancellationToken);
        return items;
    }

}

public interface ICountryLookupService
{
    Task<Domain.Models.Country?> GetByIdAsync(int ctryId, CancellationToken cancellationToken);
}

public class CountryLookupService : ICountryLookupService
{
    private readonly ICachedLookupService<Country> _repository;

    public CountryLookupService(ICachedLookupService<Country> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Domain.Models.Country?> GetByIdAsync(int ctryId, CancellationToken cancellationToken)
    {
        var items = await GetAsync(cancellationToken);
        return items.FirstOrDefault(x => x.ctry_id == ctryId)?.ToDomainModel();
    }

    private async Task<List<Country>> GetAsync(CancellationToken cancellationToken)
    {
        string key = Caching.Cache.Api.Countries(2);
        var items = await _repository.GetListAsync(key, TimeSpan.FromDays(1), cancellationToken);
        return items;
    }
}

public interface IProvinceLookupService
{
    Task<TrafficCourts.Domain.Models.Province?> GetByProvSeqNoCtryIdAsync(int provSeqNo, int ctryId, CancellationToken cancellationToken);
    Task<List<TrafficCourts.Domain.Models.Province>> GetListAsync(CancellationToken cancellationToken);
}

public class ProvinceLookupService : IProvinceLookupService
{
    private readonly ICachedLookupService<Province> _repository;
    public ProvinceLookupService(ICachedLookupService<Province> repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Models.Province?> GetByProvSeqNoCtryIdAsync(int provSeqNo, int ctryId, CancellationToken cancellationToken)
    {
        var items = await GetAsync(cancellationToken);
        return items.FirstOrDefault(x => x.prov_seq_no == provSeqNo && x.ctry_id == ctryId)?.ToDomainModel();
    }
    public async Task<List<Domain.Models.Province>> GetListAsync(CancellationToken cancellationToken)
    {
        var items = await GetAsync(cancellationToken);
        var models = items.Select(x => x.ToDomainModel()).ToList();
        return models;
    }

    private async Task<List<Province>> GetAsync(CancellationToken cancellationToken)
    {
        string key = Caching.Cache.Api.Provinces(2);
        var items = await _repository.GetListAsync(key, TimeSpan.FromDays(1), cancellationToken);
        return items;
    }
}

public interface IStatuteLookupService
{
    Task<Domain.Models.Statute> GetByIdAsync(int statuteId, CancellationToken cancellationToken);
}

public class StatuteLookupService : IStatuteLookupService
{
    private readonly ICachedLookupService<Statute> _repository;

    public StatuteLookupService(ICachedLookupService<Statute> repository)
    {
        _repository = repository;
    }

    public async Task<Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken)
    {
        var items = await GetAsync(cancellationToken);

        var index = items.BinarySearch(
            new Statute { stat_id = statuteId },
            new StatuteIdComparer());

        if (index >= 0)
        {
            return items[index].ToDomainModel(new StringBuilder());
        }

        return null;
    }

    private async Task<List<Statute>> GetAsync(CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Api.Statutes(2);
        var items = await _repository.GetListAsync(key, TimeSpan.FromHours(2), cancellationToken);
        return items;
    }
}