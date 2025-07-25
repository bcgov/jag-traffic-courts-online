using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;

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

    public async Task<ICollection<FileHistory>> GetFileHistoryForTicketAsync(string ticketNumber, TimeZoneInfo timeZone, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var history = await _oracleDataApi.GetFileHistoryByTicketNumberAsync(ticketNumber, cancellationToken);

        return history;
    }
}
