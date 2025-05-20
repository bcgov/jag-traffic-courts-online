using Microsoft.Extensions.Logging;

namespace TrafficCourts.OrdsDataService.Occam;

public interface IOccamDisputeWithUpdateRequestRepository
{
    Task<OrdsDataServicePagedCollectionResponse<OccamDisputeWithUpdateRequest>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken);
}

internal class OccamDisputeWithUpdateRequestRepository : OccamOrdsRepository<OccamDisputeWithUpdateRequestRepository>, IOccamDisputeWithUpdateRequestRepository
{
    public OccamDisputeWithUpdateRequestRepository(OccamOrdsDataServiceClient client, ILogger<OccamDisputeWithUpdateRequestRepository> logger)
        : base(client, "/v2/occam_dispute_update_requests_list", logger)
    { }

    public async Task<OrdsDataServicePagedCollectionResponse<OccamDisputeWithUpdateRequest>> GetListAsync(
        IReadOnlyDictionary<string, string>? parameters,
        CancellationToken cancellationToken)
    {
        var jsonTypeInfo = JsonContext.Default.OrdsDataServicePagedCollectionResponseOccamDisputeWithUpdateRequest;

        var response = await GetPagedListAsync(
            parameters,
            JsonContext.Default.OrdsDataServicePagedCollectionResponseOccamDisputeWithUpdateRequest,
            ETagCache.FiveMinutes,
            cancellationToken);

        return response;
    }
}
