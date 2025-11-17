using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for EmailHistoryService
/// </summary>
public class EmailHistoryService : IEmailHistoryService
{
    private readonly IOracleDataApiService _oracleDataApi;

    public EmailHistoryService(IOracleDataApiService oracleDataApi)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
    }

    public async Task<ICollection<EmailHistory>> GetEmailHistoryForTicketAsync(string ticketNumber, TimeZoneInfo timeZone, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var history = await _oracleDataApi.GetEmailHistoryByTicketNumberAsync(ticketNumber, cancellationToken);

        history.UtcToLocalTime(timeZone);

        return history;
    }
}
