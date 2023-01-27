using TrafficCourts.Common.Models;
using TrafficCourts.Coms.Client;

namespace TrafficCourts.Staff.Service.Services;

public interface IComsService
{
    /// <summary>
    /// Saves the given file object with optional content type and metadata to object store through COMS service
    /// </summary>
    /// <param name="file"></param>
    /// <param name="metadata"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of newly inserted file to the object storage</returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="file"/> has a null data property.</exception>
    /// <exception cref="MetadataInvalidKeyException">A key contains an invalid character</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata is too long</exception>
    /// <exception cref="TagKeyEmptyException"></exception>
    /// <exception cref="TagKeyTooLongException"></exception>
    /// <exception cref="TagValueTooLongException"></exception>
    /// <exception cref="TooManyTagsException"></exception>
    /// <exception cref="ObjectManagementServiceException">Other error.</exception>
    Task<Guid> SaveFileAsync(IFormFile file, Dictionary<string, string> metadata, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a file with data and details through COMS service for the given unique file ID
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>COMS File Object</returns>
    /// <exception cref="ObjectManagementServiceException">Unable to return file through COMS</exception>
    Task<Coms.Client.File> GetFileAsync(Guid fileId, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the specified file through COMS service for the given unique file ID
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ObjectManagementServiceException">Unable to delete the file through COMS</exception>
    Task DeleteFileAsync(Guid fileId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the IDs of the documents found in object storage through COMS service based on the search parameters provided
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="tags"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ObjectManagementServiceException">There was an error searching files in COMS</exception>
    /// <returns></returns>
    Task<List<FileMetadata>> GetFilesBySearchAsync(IDictionary<string, string>? metadata, IDictionary<string, string>? tags, CancellationToken cancellationToken);
}
