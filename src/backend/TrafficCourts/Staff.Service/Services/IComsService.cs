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

}
