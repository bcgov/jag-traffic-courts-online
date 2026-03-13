using System.Security.Claims;
using TrafficCourts.Coms.Client;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Staff.Service.Services;

public interface IStaffDocumentService
{
    /// <summary>
    /// Saves the given file object with optional content type and metadata to object store through COMS service
    /// </summary>
    /// <param name="file"></param>
    /// <param name="properties">The properties to add to the document</param>
    /// <param name="user">The user creating the file.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Metadata of the newly inserted file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="file"/> has a null data property.</exception>
    /// <exception cref="MetadataInvalidKeyException">A key contains an invalid character</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata is too long</exception>
    /// <exception cref="TagKeyEmptyException"></exception>
    /// <exception cref="TagKeyTooLongException"></exception>
    /// <exception cref="TagValueTooLongException"></exception>
    /// <exception cref="TooManyTagsException"></exception>
    /// <exception cref="ObjectManagementServiceException">Other error.</exception>
    Task<FileMetadata> SaveFileAsync(IFormFile file, Domain.Models.DocumentProperties properties, ClaimsPrincipal user, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a file with data and details through COMS service for the given unique file ID
    /// </summary>
    /// <param name="fileId">The document identifier.</param>
    /// <param name="checkVirusScan">Whether to validate the file has passed the virus scan.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>COMS File Object</returns>
    /// <exception cref="ObjectManagementServiceException">Unable to return file through COMS</exception>
    Task<Coms.Client.File> GetFileAsync(Guid fileId, bool checkVirusScan, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a given document with the specified properties.
    /// </summary>
    /// <param name="fileId">The document identifier.</param>
    /// <param name="updatedProperties">The new document properties.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The fileId or the file data is missing.</exception>
    /// <exception cref="MetadataInvalidKeyException">A key contains an invalid character</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata is too long</exception>
    /// <exception cref="TagKeyEmptyException"></exception>
    /// <exception cref="TagKeyTooLongException"></exception>
    /// <exception cref="TagValueTooLongException"></exception>
    /// <exception cref="TooManyTagsException"></exception>
    /// <exception cref="ObjectManagementServiceException">Error executing the service call.</exception>
    Task UpdateFileAsync(Guid fileId, DocumentProperties updatedProperties, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the specified file through COMS service for the given unique file ID
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="user">The user deleting the file.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ObjectManagementServiceException">Unable to delete the file through COMS</exception>
    Task DeleteFileAsync(Guid fileId, ClaimsPrincipal user, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the IDs of the documents found in object storage through COMS service based on the search parameters provided
    /// </summary>
    /// <param name="properties">The document properties to search on.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ObjectManagementServiceException">There was an error searching files in COMS</exception>
    /// <returns></returns>
    Task<List<TrafficCourts.Domain.Models.FileMetadata>> FindFilesAsync(Domain.Models.DocumentProperties properties, CancellationToken cancellationToken);
}
