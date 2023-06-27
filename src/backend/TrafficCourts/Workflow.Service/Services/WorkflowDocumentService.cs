using TrafficCourts.Common.Models;
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
        _logger.LogDebug("Getting the file {FileId} through COMS", fileId);

        return await _objectManagementService.GetFileAsync(fileId, cancellationToken);
    }

    public async Task RemoveFileAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Removing the file {FileId} through COMS", fileId);

        await _objectManagementService.DeleteFileAsync(fileId, cancellationToken);
        return;
    }

    public async Task SaveDocumentPropertiesAsync(Guid id, DocumentProperties properties, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cannot replace save document properties on empty object id", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(properties);

        _logger.LogDebug("Updating the file {FileId} properties through COMS", id);

        var tags = properties.ToTags();

        await _objectManagementService.SetTagsAsync(id, tags, cancellationToken);
    }
}
