using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for FileHistoryService
/// </summary>
public class FileHistoryService : IFileHistoryService
{
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileHistoryService(
        IOracleDataApiClient oracleDataApi,
        IHttpContextAccessor httpContextAccessor,
        ILogger<FileHistoryService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<ICollection<FileHistory>> GetFileHistoryForTicketAsync(String ticketNumber, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetFileHistoryByTicketNumberAsync(ticketNumber, cancellationToken);
    }
}
