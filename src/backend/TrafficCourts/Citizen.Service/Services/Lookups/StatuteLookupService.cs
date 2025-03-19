using Serilog;
using System.Text;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;

using ILogger = Serilog.ILogger;

namespace TrafficCourts.Citizen.Service.Services.Lookups;

public class StatuteLookupService : IStatuteLookupService
{
    private readonly IStatuteRepository _repository;
    private readonly ILogger _logger;

    public StatuteLookupService(IStatuteRepository repository, ILogger logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger;
    }

    public async Task<TrafficCourts.Domain.Models.Statute?> GetBySectionAsync(string section, CancellationToken cancellationToken)
    {
        // this is expensive to fetch all the records. Ideally we would split the section into the act code, section code, etc to find
        // the matching record. For now, we will fetch all the records and find the matching record.
        var models = await GetListAsync(cancellationToken);
        return models.FirstOrDefault(x => x.Code == section);
    }

    public async Task<TrafficCourts.Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken)
    {
        List<Statute> items = await _repository.GetListAsync(cancellationToken);

        var model = items.SingleOrDefault(x => x.stat_id == statuteId);

        if (model is null)
        {
            _logger.Information("Statute with ID {StatuteId} not found", statuteId);
            return null;
        }
        else
        {
            return model.ToDomainModel(new StringBuilder());
        }
    }

    public async Task<IList<TrafficCourts.Domain.Models.Statute>> GetListAsync(CancellationToken cancellationToken)
    {
        var models = await GetListAsync(DateTime.Today, cancellationToken);
        return models;
    }

    public async Task<IList<TrafficCourts.Domain.Models.Statute>> GetListAsync(DateTime effectiveOn, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(effectiveOn, cancellationToken);
        var buffer = new StringBuilder();
        var models = items.Select(x => x.ToDomainModel(buffer)).ToList();
        return models;
    }
}