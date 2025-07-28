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

        var result = response?.Rows?
            .Where(x =>
            x.cdln_active_yn.Equals("Y", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(x.cdln_language_dsc, "Chinese", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(x.cdln_language_dsc, "Other", StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new List<Language>();

        return result;
    }
}
