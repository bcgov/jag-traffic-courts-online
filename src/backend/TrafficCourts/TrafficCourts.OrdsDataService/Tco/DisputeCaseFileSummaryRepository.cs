using Microsoft.Extensions.Logging;

namespace TrafficCourts.OrdsDataService.Tco;

public interface IDisputeCaseFileSummaryRepository
{
    Task<OrdsDataServicePagedCollectionResponse<OrdsDisputeCaseFileSummary>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken);
}

internal class DisputeCaseFileSummaryRepository : TcoOrdsRepository<DisputeCaseFileSummaryRepository>, IDisputeCaseFileSummaryRepository
{
    public DisputeCaseFileSummaryRepository(TcoOrdsDataServiceClient client, ILogger<DisputeCaseFileSummaryRepository> logger) 
        : base(client, "/v2/tco_disputes", logger)
    {
    }

    public async Task<OrdsDataServicePagedCollectionResponse<OrdsDisputeCaseFileSummary>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken)
    {
        var jsonTypeInfo = JsonContext.Default.OrdsDataServicePagedCollectionResponseOrdsDisputeCaseFileSummary;

        var response = await GetPagedListAsync(parameters, jsonTypeInfo, ETagCache.FiveMinutes, cancellationToken);

        return response;
    }
}
