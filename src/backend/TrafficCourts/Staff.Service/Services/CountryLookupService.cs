using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Services;

public class CountryLookupService : ICountryLookupService
{
    private readonly ICountryRepository _repository;

    public CountryLookupService(ICountryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Domain.Models.Country?> GetByIdAsync(int ctryId, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var model = items
            .Where(_ => _.ctry_id == ctryId)
            .Select(ToDomainModel)
            .FirstOrDefault();

        return model;
    }

    public async Task<List<Domain.Models.Country>> GetListAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);
        var models = items.Select(ToDomainModel).ToList();
        return models;
    }

    private Domain.Models.Country ToDomainModel(Country item)
    {
        return new Domain.Models.Country
        (
            item.ctry_id.ToString(),
            item.ctry_long_nm
        );
    }
}
