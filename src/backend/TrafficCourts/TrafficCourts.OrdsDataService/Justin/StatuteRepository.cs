using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace TrafficCourts.OrdsDataService.Justin;

internal class StatuteRepository : OrdsRepository<StatuteRepository>, IStatuteRepository
{
    public StatuteRepository(OrdsDataServiceClient client, ILogger<StatuteRepository> logger)
        : base(client, "/v2/justin_statutes", logger)
    {
    }

    public async Task<Statute?> GetAsync(int stat_id, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "stat_id_eq", stat_id.ToString() }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseStatute,
            ETagCache.OneHour,
            cancellationToken);

        return response?.Rows?.FirstOrDefault();
    }

    public async Task<List<Statute>> GetListAsync(CancellationToken cancellationToken = default)
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

    public async Task<List<Statute>> GetListAsync(DateTime effectiveOn, CancellationToken cancellationToken = default)
    {
        // when we filter stat_termination_dt >= :date, the back end will actually do
        // :date <= stat_termination_dt OR stat_termination_dt is null
        // since we want only the statutes that are effective on the given date
        var date = effectiveOn.ToString("yyyy-MM-dd");
        var parameters = new Dictionary<string, string>
        {
            { "act_cd_in", "MVA,MVR" },
            { "stat_effective_dt_le", date },
            { "stat_termination_dt_ge", date }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseStatute,
            ETagCache.OneDay,
            cancellationToken);

        return response?.Rows ?? [];
    }
}
