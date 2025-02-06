using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Services;

public class AgencyLookupService : IAgencyLookupService
{
    private readonly IAgencyRepository _repository;

    public AgencyLookupService(IAgencyRepository repository)
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

    private Task<List<Agency>> GetAsync(CancellationToken cancellationToken) => _repository.GetListAsync(cancellationToken);
}
