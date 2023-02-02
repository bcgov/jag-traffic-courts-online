using MassTransit;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for EmailHistoryService
/// </summary>
public class EmailHistoryService : IEmailHistoryService
{
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly ILogger<EmailHistoryService> _logger;
    private readonly IFilePersistenceService _filePersistenceService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public EmailHistoryService(
        IOracleDataApiClient oracleDataApi,
        IFilePersistenceService filePersistenceService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<EmailHistoryService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<EmailHistory>> GetEmailHistoryForTicketAsync(String ticketNumber, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetEmailHistoryByTicketNumberAsync(ticketNumber, cancellationToken);
    }
}
