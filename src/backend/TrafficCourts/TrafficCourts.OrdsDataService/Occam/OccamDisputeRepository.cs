using Microsoft.Extensions.Logging;

namespace TrafficCourts.OrdsDataService.Occam;

public interface IOccamDisputeRepository
{
    Task<List<OccamDispute>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken);
}

internal class OccamDisputeRepository : OccamOrdsRepository<OccamDisputeRepository>, IOccamDisputeRepository
{
    public OccamDisputeRepository(OccamOrdsDataServiceClient client, ILogger<OccamDisputeRepository> logger) 
        //: base(client, "/v2/occam_disputes", logger)
        : base(client, "api/lookup/dispute-case-file-statuses/v2", logger)
    {
    }

    public async Task<List<OccamDispute>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken)
    {
        var response = await GetPagedListAsync(
            parameters,
            JsonContext.Default.OrdsDataServicePagedCollectionResponseOccamDispute,
            ETagCache.FiveMinutes,
            cancellationToken);

        return response?.Rows ?? [];
    }
}
