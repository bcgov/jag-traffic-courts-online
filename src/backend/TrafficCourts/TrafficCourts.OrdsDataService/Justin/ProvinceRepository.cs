using Microsoft.Extensions.Logging;
using TrafficCourts.OrdsDataService.Tco;

namespace TrafficCourts.OrdsDataService.Justin;

internal class ProvinceRepository : TcoOrdsRepository<ProvinceRepository>, IProvinceRepository
{
    public ProvinceRepository(TcoOrdsDataServiceClient client, ILogger<ProvinceRepository> logger) 
        : base(client, "/v2/justin_provinces", logger)
    {
    }

    public async Task<List<Province>> GetListAsync(CancellationToken cancellationToken)
    {
        var response = await GetListAsync(
            parameters: null,
            JsonContext.Default.OrdsDataServiceCollectionResponseProvince,
            ETagCache.OneDay,
            cancellationToken);

        return response?.Rows ?? [];
    }
}
