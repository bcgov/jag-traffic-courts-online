using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Services;

public class ProvinceLookupService : IProvinceLookupService
{
    private readonly IProvinceRepository _repository;

    public ProvinceLookupService(IProvinceRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Domain.Models.Province?> GetByProvSeqNoCtryIdAsync(int provSeqNo, int ctryId, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);
        return items.FirstOrDefault(x => x.prov_seq_no == provSeqNo && x.ctry_id == ctryId)?.ToDomainModel();
    }
    public async Task<List<Domain.Models.Province>> GetListAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);
        var models = items.Select(x => x.ToDomainModel()).ToList();
        return models;
    }
}
