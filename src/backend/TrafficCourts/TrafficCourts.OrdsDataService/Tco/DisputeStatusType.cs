using Microsoft.Extensions.Logging;

namespace TrafficCourts.OrdsDataService.Tco;

public class DisputeStatusType
{
#pragma warning disable IDE1006 // Naming Styles
    public string dispute_status_type_cd { get; set; } = string.Empty;
    public string dispute_status_type_dsc { get; set; } = string.Empty;
    public string active_yn { get; set; } = string.Empty;
#pragma warning restore IDE1006 // Naming Styles
}

public interface IDisputeStatusTypeRepository
{
    Task<List<DisputeStatusType>> GetListAsync(CancellationToken cancellationToken);
}

internal class DisputeStatusTypeRepository : TcoOrdsRepository<DisputeStatusTypeRepository>, IDisputeStatusTypeRepository
{
    public DisputeStatusTypeRepository(TcoOrdsDataServiceClient client, ILogger<DisputeStatusTypeRepository> logger)
        : base(client, "/v2/tco_dispute_status_types", logger)
    {
    }

    public async Task<List<DisputeStatusType>> GetListAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, string> parameters = new()
        {
            { "active_yn_eq", "Y" }
        };

        var response = await GetListAsync(
            parameters,
            JsonContext.Default.OrdsDataServiceCollectionResponseDisputeStatusType,
            ETagCache.OneDay,
            cancellationToken);

        return response?.Rows ?? [];
    }
}
