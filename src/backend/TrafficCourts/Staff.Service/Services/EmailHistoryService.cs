using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for EmailHistoryService
/// </summary>
public class EmailHistoryService : IEmailHistoryService
{
    private readonly IOracleDataApiClient _oracleDataApi;


    public EmailHistoryService(
        IOracleDataApiClient oracleDataApi,
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
