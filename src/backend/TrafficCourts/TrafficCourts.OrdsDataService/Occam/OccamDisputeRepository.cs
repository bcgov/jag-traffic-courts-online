using Microsoft.Extensions.Logging;

namespace TrafficCourts.OrdsDataService.Occam;

public interface IOccamDisputeRepository
{
    Task<OrdsDataServicePagedCollectionResponse<OccamDispute>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken);
}

internal class OccamDisputeRepository : OccamOrdsRepository<OccamDisputeRepository>, IOccamDisputeRepository
{
    public OccamDisputeRepository(OccamOrdsDataServiceClient client, ILogger<OccamDisputeRepository> logger)
        : base(client, "/v2/occam_dispute_list", logger)
    { }

    public async Task<OrdsDataServicePagedCollectionResponse<OccamDispute>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken)
    {
        var jsonTypeInfo = JsonContext.Default.OrdsDataServicePagedCollectionResponseOccamDispute;

        var response = await GetPagedListAsync(
            parameters,
            JsonContext.Default.OrdsDataServicePagedCollectionResponseOccamDispute,
            ETagCache.FiveMinutes,
            cancellationToken);

        return response;
    }
}
