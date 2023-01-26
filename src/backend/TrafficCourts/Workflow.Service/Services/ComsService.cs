using TrafficCourts.Coms.Client;

namespace TrafficCourts.Workflow.Service.Services;

/// <summary>
/// A service for file operations utilizing common object management service client
/// </summary>
public class ComsService : IComsService
{
    private readonly IObjectManagementService _objectManagementService;
    private readonly ILogger<ComsService> _logger;

    public ComsService(
        IObjectManagementService objectManagementService,
        ILogger<ComsService> logger)
    {
        _objectManagementService = objectManagementService ?? throw new ArgumentNullException(nameof(objectManagementService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Coms.Client.File> GetFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting the file through COMS");

        return await _objectManagementService.GetFileAsync(fileId, false, cancellationToken);
    }

    public async Task UpdateFileAsync(Guid id, Coms.Client.File file, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the file through COMS");

        await _objectManagementService.UpdateFileAsync(id, file, cancellationToken);
    }
}
