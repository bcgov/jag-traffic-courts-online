using Microsoft.Extensions.Logging;
using TrafficCourts.OrdsDataService.Tco;

namespace TrafficCourts.OrdsDataService.Justin;

internal class StatuteRepository : TcoOrdsRepository<StatuteRepository>, IStatuteRepository
{
    public StatuteRepository(TcoOrdsDataServiceClient client, ILogger<StatuteRepository> logger)
        : base(client, "/v2/justin_statutes", logger)
    {
    }

    public async Task<Statute?> GetAsync(int stat_id, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "stat_id", stat_id.ToString() }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseStatute,
            ETagCache.OneHour,
            cancellationToken);

        return response?.Rows?.FirstOrDefault();
    }

    public async Task<List<Statute>> GetListAsync(CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "act_cd_in", "MVA,MVR" }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseStatute,
            ETagCache.OneDay,
            cancellationToken);

        return response?.Rows ?? [];
    }
}
