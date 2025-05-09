using Microsoft.Extensions.Logging;
using TrafficCourts.OrdsDataService.Tco;

namespace TrafficCourts.OrdsDataService.Justin;

internal class CountryRepository : OrdsRepository<CountryRepository>, ICountryRepository
{
    public CountryRepository(TcoOrdsDataServiceClient client, ILogger<CountryRepository> logger) 
        : base(client, "/v2/justin_countries", logger)
    {
    }

    public async Task<List<Country>> GetListAsync(CancellationToken cancellationToken)
    {
        var response = await GetListAsync(
            parameters: null,
            JsonContext.Default.OrdsDataServiceCollectionResponseCountry,
            ETagCache.OneDay,
            cancellationToken);

        return response?.Rows ?? [];
    }
}
