using System.Text;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Services;

public class StatuteLookupService : IStatuteLookupService
{
    private readonly IStatuteRepository _repository;

    public StatuteLookupService(IStatuteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken)
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
}