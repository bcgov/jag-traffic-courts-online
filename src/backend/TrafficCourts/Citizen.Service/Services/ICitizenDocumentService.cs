using TrafficCourts.Citizen.Service.Models;
using TrafficCourts.Common.Models;
using TrafficCourts.Coms.Client;

namespace TrafficCourts.Citizen.Service.Services;

public interface ICitizenDocumentService
{
    /// <summary>
    /// Saves the given file object with optional content type and metadata to object store through COMS service
    /// </summary>
    /// <param name="base64FileString"></param>
    /// <param name="fileName"></param>
    /// <param name="properties">The document properties</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of newly inserted file to the object storage</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> has a null data property.</exception>
    /// <exception cref="MetadataInvalidKeyException">A key contains an invalid character</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata is too long</exception>
    /// <exception cref="TagKeyTooLongException"></exception>
    /// <exception cref="TagValueTooLongException"></exception>
    /// <exception cref="TooManyTagsException"></exception>
    /// <exception cref="ObjectManagementServiceException">Other error.</exception>
    Task<Guid> SaveFileAsync(string base64FileString, string fileName, DocumentProperties properties, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the specified file through COMS service for the given unique file ID
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ObjectManagementServiceException">Unable to delete the file through COMS</exception>
    Task DeleteFileAsync(Guid fileId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a dictionary of IDs and file names of the documents found in object storage through COMS service based on the search parameters provided
    /// </summary>
    /// <param name="properties">The document properties to search on.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException">Parameters is null.</exception>
    /// <exception cref="MetadataInvalidKeyException">An invalid metadata key was supplied.</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata was too long.</exception>
    /// <exception cref="TooManyTagsException">Too many tags were supplied. Only 10 tags are allowed.</exception>
    /// <exception cref="TagKeyTooLongException">A tag key was too long. Maximum length of a tag key is 128.</exception>
    /// <exception cref="TagValueTooLongException">A tag value was too long. Maximum length of a tag value is 256.</exception>
    /// <exception cref="ObjectManagementServiceException">There was an error searching files in COMS</exception>
    /// <returns></returns>
    Task<List<FileMetadata>> FindFilesAsync(DocumentProperties properties, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a file with data and details through COMS service for the given unique file ID
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>COMS File Object</returns>
    /// <exception cref="FileNotFoundException">File not found</exception>
    /// <exception cref="ObjectManagementServiceException">Unable to return file through COMS</exception>
    Task<Coms.Client.File> GetFileAsync(Guid fileId, CancellationToken cancellationToken);
}
