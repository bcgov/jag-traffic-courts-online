using TrafficCourts.Common.Models;
using TrafficCourts.Coms.Client;

namespace TrafficCourts.Workflow.Service.Services;

public interface IWorkflowDocumentService
{
    /// <summary>
    /// Retrieves a file with data and details through COMS service for the given unique file ID
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>COMS File Object</returns>
    /// <exception cref="ObjectManagementServiceException">Unable to return file through COMS</exception>
    Task<Coms.Client.File> GetFileAsync(Guid fileId, CancellationToken cancellationToken);

    /// <summary>
    /// Sets the metadata on the specified file.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="meta"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveDocumentPropertiesAsync(Guid id, DocumentProperties properties, CancellationToken cancellationToken);
}
