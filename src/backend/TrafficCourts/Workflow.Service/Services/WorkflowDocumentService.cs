using TrafficCourts.Coms.Client;

namespace TrafficCourts.Workflow.Service.Services;

/// <summary>
/// A service for file operations utilizing common object management service client
/// </summary>
public class WorkflowDocumentService : IWorkflowDocumentService
{
    private readonly IObjectManagementService _objectManagementService;
    private readonly ILogger<WorkflowDocumentService> _logger;

    public WorkflowDocumentService(
        IObjectManagementService objectManagementService,
        ILogger<WorkflowDocumentService> logger)
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

    public async Task SetFileMetadataAsync(Guid id, IReadOnlyDictionary<string, string> meta, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot replace metadata on empty object id", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(meta);

        _logger.LogDebug("Updating the file metadata through COMS");

        await _objectManagementService.ReplaceMetadataAsync(id, meta, cancellationToken);


    }
}
