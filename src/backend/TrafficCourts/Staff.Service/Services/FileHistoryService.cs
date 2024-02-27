using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for FileHistoryService
/// </summary>
public class FileHistoryService : IFileHistoryService
{
    private readonly IOracleDataApiService _oracleDataApi;

    public FileHistoryService(IOracleDataApiService oracleDataApi)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
    }

    public async Task<ICollection<FileHistory>> GetFileHistoryForTicketAsync(String ticketNumber, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetFileHistoryByTicketNumberAsync(ticketNumber, cancellationToken);
    }
}
