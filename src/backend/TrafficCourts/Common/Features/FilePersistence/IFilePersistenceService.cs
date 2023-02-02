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
    /// Serializes generic object data into a JSON object and saves it to the persistence location with object type, created date (meta data) within the headers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The unique file name can be used to retrieve or reference.</returns>
    Task<string> SaveJsonFileAsync<T>(T data, string filename, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the file by name.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">The file was not found.</exception>
    Task<MemoryStream> GetFileAsync(string filename, CancellationToken cancellationToken);

    /// <summary>
    /// Gets generic deserialized json object by filename if target deserialization object type matches.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filename"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> GetJsonDataAsync<T>(string filename, CancellationToken cancellationToken) where T : class;
}
