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
        var item = await _repository.GetAsync(statuteId, cancellationToken);

        if (item is not null)
        {
            return item.ToDomainModel(new StringBuilder());
        }

        return null;
    }
}