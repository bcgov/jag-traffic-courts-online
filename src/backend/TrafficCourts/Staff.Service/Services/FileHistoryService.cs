using MassTransit;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// Summary description for FileHistoryService
/// </summary>
public class FileHistoryService : IFileHistoryService
{
    private readonly IOracleDataApiClient _oracleDataApi;
    private readonly ILogger<FileHistoryService> _logger;
    private readonly IBus _bus;
    private readonly IFilePersistenceService _filePersistenceService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public FileHistoryService(
        IOracleDataApiClient oracleDataApi,
        IBus bus,
        IFilePersistenceService filePersistenceService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<FileHistoryService> logger)
    {
        _oracleDataApi = oracleDataApi ?? throw new ArgumentNullException(nameof(oracleDataApi));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _filePersistenceService = filePersistenceService ?? throw new ArgumentNullException(nameof(filePersistenceService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<FileHistory>> GetFileHistoryForTicketAsync(String ticketNumber, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.GetFileHistoryByTicketNumberAsync(ticketNumber, cancellationToken);
    }

    public async Task<long> SaveFileHistoryAsync(FileHistory fileHistory, CancellationToken cancellationToken)
    {
        return await _oracleDataApi.InsertFileHistoryAsync(fileHistory, cancellationToken);
    }
}
