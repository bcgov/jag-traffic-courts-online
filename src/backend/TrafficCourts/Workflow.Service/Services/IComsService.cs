using TrafficCourts.Coms.Client;

namespace TrafficCourts.Workflow.Service.Services;

public interface IComsService
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
    /// Updates a file in the object management service
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
    /// <exception cref="MetadataInvalidKeyException">A key contains an invalid character</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata is too long</exception>
    /// <exception cref="TagKeyEmptyException"></exception>
    /// <exception cref="TagKeyTooLongException"></exception>
    /// <exception cref="TagValueTooLongException"></exception>
    /// <exception cref="TooManyTagsException"></exception>
    /// <exception cref="ObjectManagementServiceException">Error executing the service call.</exception>
    Task UpdateFileAsync(Guid id, Coms.Client.File file, CancellationToken cancellationToken);
}
