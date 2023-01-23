namespace TrafficCourts.Coms.Client;

public interface IObjectManagementService
{
    /// <summary>
    /// Creates a file in the object management service.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="file"/> has a null data property.</exception>
    /// <exception cref="MetadataInvalidKeyException">An invalid metadata key was supplied.</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata was too long.</exception>
    /// <exception cref="TooManyTagsException">Too many tags were supplied. Only 10 tags are allowed.</exception>
    /// <exception cref="TagKeyTooLongException">A tag key was too long. Maximum length of a tag key is 128.</exception>
    /// <exception cref="TagValueTooLongException">A tag value was too long. Maximum length of a tag value is 256.</exception>
    /// <exception cref="ObjectManagementServiceException">Error executing the service call.</exception>
    Task<Guid> CreateFileAsync(File file, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a file in the object management service.
    /// </summary>
    /// <param name="id"></param>
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
    Task UpdateFileAsync(Guid id, File file, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a file from the object management service.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ObjectManagementServiceException">Error executing the service call.</exception>
    Task DeleteFileAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeTags"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ObjectManagementServiceException"></exception>
    Task<File> GetFileAsync(Guid id, bool includeTags, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is null.</exception>
    /// <exception cref="MetadataInvalidKeyException">An invalid metadata key was supplied.</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata was too long.</exception>
    /// <exception cref="TooManyTagsException">Too many tags were supplied. Only 10 tags are allowed.</exception>
    /// <exception cref="TagKeyTooLongException">A tag key was too long. Maximum length of a tag key is 128.</exception>
    /// <exception cref="TagValueTooLongException">A tag value was too long. Maximum length of a tag value is 256.</exception>
    /// <exception cref="ObjectManagementServiceException">Other error occured</exception>
    Task<IList<FileSearchResult>> FileSearchAsync(FileSearchParameters parameters, CancellationToken cancellationToken);
}
