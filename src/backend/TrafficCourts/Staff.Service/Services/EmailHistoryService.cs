using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for EmailHistoryService
/// </summary>
public class EmailHistoryService : IEmailHistoryService
{
    private readonly IOracleDataApiService _oracleDataApi;


    public EmailHistoryService(
        IOracleDataApiService oracleDataApi,
        IHttpContextAccessor httpContextAccessor,
        ILogger<EmailHistoryService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
    }

    public async Task<ICollection<EmailHistory>> GetEmailHistoryForTicketAsync(String ticketNumber, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetEmailHistoryByTicketNumberAsync(ticketNumber, cancellationToken);
    }
}
