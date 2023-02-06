namespace TrafficCourts.Coms.Client;

/// <summary>
/// Abstraction of generated <see cref="ObjectManagementClient"/>
/// </summary>
public interface IObjectManagementClient
{
    string BaseUrl { get; set; }
    bool ReadResponseAsString { get; set; }

    #region Metadata
    Task AddMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
        
    Task DeleteMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);

    /// <summary>
    /// Get the metadata for a list of documents.
    /// </summary>
    /// <param name="ids">The list of documents to fetch metadata for</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<ObjectMetadata>> GetObjectMetadataAsync(IList<Guid> ids, CancellationToken cancellationToken);
    /// <summary>
    /// Get the metadata for a single document.
    /// </summary>
    /// <param name="id">The id of document to fetch metadata for</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ObjectMetadata?> GetObjectMetadataAsync(Guid id, CancellationToken cancellationToken);

    Task ReplaceMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);

    #endregion

    #region Tagging
    Task AddTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task DeleteTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task ReplaceTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    #endregion

    #region Object
    Task<List<Anonymous>> CreateObjectsAsync(IReadOnlyDictionary<string, string>? meta, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken);
    Task<FileResponse> ReadObjectAsync(Guid objId, DownloadMode? download, int? expiresIn, string? versionId, CancellationToken cancellationToken);

    Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken);
    Task HeadObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken);

    Task<List<DBObject>> SearchObjectsAsync(IReadOnlyDictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? deleteMarker, bool? latest, bool? @public, string? mimeType, string? name, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken);

    Task<Response> UpdateObjectAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken);
    #endregion

    #region Versions
    Task<S3VersionList> ListObjectVersionAsync(Guid objId, CancellationToken cancellationToken);
    #endregion
}
