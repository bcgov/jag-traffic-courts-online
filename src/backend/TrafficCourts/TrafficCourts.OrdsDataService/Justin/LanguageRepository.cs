using Microsoft.Extensions.Logging;
using TrafficCourts.OrdsDataService.Tco;

namespace TrafficCourts.OrdsDataService.Justin;

internal class LanguageRepository : OrdsRepository<LanguageRepository>, ILanguageRepository
{
    public LanguageRepository(TcoOrdsDataServiceClient client, ILogger<LanguageRepository> logger) 
        : base(client, "/v2/justin_languages", logger)
    {
    }

    public async Task<List<Language>> GetListAsync(CancellationToken cancellationToken)
    {
        var response = await GetListAsync(
            parameters: null,
            JsonContext.Default.OrdsDataServiceCollectionResponseLanguage,
            ETagCache.OneDay,
            cancellationToken);

        return response?.Rows?
        .Where(x => x.cdln_active_yn.Equals("Y", StringComparison.OrdinalIgnoreCase))
        .ToList() ?? [];
    }
}
