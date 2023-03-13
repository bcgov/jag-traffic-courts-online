namespace TrafficCourts.Coms.Client;

/// <summary>
/// Abstraction of generated <see cref="ObjectManagementClient"/>
/// </summary>
public interface IObjectManagementClient
{
    string BaseUrl { get; set; }
    bool ReadResponseAsString { get; set; }

    #region Metadata
    Task<IList<Anonymous2>> FetchMetadataAsync(IList<Guid>? ids, IReadOnlyDictionary<string, string>? meta, CancellationToken cancellationToken);

    Task AddMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
        
    Task DeleteMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);

    Task ReplaceMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);

    #endregion

    #region Tagging
    Task<IList<Anonymous3>> FetchTagsAsync(IList<Guid>? ids, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken);
    Task AddTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task DeleteTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task ReplaceTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);

    #endregion

    #region Object
    Task<List<Anonymous>> CreateObjectsAsync(IReadOnlyDictionary<string, string>? meta, IReadOnlyDictionary<string, string>? tags, Guid? bucketId, FileParameter anyKey, CancellationToken cancellationToken);
    
    Task<FileResponse> ReadObjectAsync(Guid objId, DownloadMode? download, int? expiresIn, string? versionId, CancellationToken cancellationToken);

    Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken);
    Task HeadObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken);

    Task<List<DBObject>> SearchObjectsAsync(IReadOnlyDictionary<string, string>? meta, IList<Guid>? ids, Guid? bucketId, string? path, bool? active, bool? deleteMarker, bool? latest, bool? @public, string? mimeType, string? name, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken);

    Task<Response> UpdateObjectAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken);
    #endregion

    #region Versions
    Task<List<DBVersion>> ListObjectVersionAsync(Guid objId, CancellationToken cancellationToken);
    #endregion
}
