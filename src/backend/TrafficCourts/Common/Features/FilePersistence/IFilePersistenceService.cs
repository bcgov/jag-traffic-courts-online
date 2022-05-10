namespace TrafficCourts.Common.Features.FilePersistence;

/// <summary>
/// Provides a service to persist or read a file.
/// </summary>
public interface IFilePersistenceService
{
    /// <summary>
    /// Saves the data in the <see cref="MemoryStream"/> to the persistence location.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The unique file name can be used to retrieve or reference.</returns>
    Task<string> SaveFileAsync(MemoryStream data, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the file by name.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">The file was not found.</exception>
    Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken);
    /// <summary>
    /// Removes the file from storage.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteFileAsync(string filename, CancellationToken cancellationToken);
}
