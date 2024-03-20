using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for FileHistoryService
/// </summary>
public class FileHistoryService : IFileHistoryService
{
    private readonly IOracleDataApiService _oracleDataApi;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileHistoryService(
        IOracleDataApiService oracleDataApi,
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
