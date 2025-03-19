using System.Text;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Citizen.Service.Services.Lookups;

public class StatuteLookupService : IStatuteLookupService
{
    private readonly IStatuteRepository _repository;

    public StatuteLookupService(IStatuteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<TrafficCourts.Domain.Models.Statute?> GetBySectionAsync(string section, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var buffer = new StringBuilder();
        var models = items.Select(_ => _.ToDomainModel(buffer)).ToList();

        return models.FirstOrDefault(x => x.Code == section);
    }

    public async Task<TrafficCourts.Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var index = items.BinarySearch(
            new Statute { stat_id = statuteId },
            new StatuteIdComparer());

        if (index >= 0)
        {
            return items[index].ToDomainModel(new StringBuilder());
        }

        return null;
    }

    public async Task<IList<TrafficCourts.Domain.Models.Statute>> GetListAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var buffer = new StringBuilder();

        var models = items.Select(x => x.ToDomainModel(buffer)).ToList();

        return models;
    }
}