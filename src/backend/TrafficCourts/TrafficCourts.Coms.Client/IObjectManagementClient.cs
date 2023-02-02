namespace TrafficCourts.Coms.Client;

/// <summary>
/// Abstraction of generated <see cref="ObjectManagementClient"/>
/// </summary>
public interface IObjectManagementClient
{
    string BaseUrl { get; set; }
    bool ReadResponseAsString { get; set; }

    Task AddMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId);
    Task AddMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
    Task<List<DBObjectPermission>> AddPermissionsAsync(Guid objId, IEnumerable<RequestPermissionTuple>? body);
    Task<List<DBObjectPermission>> AddPermissionsAsync(Guid objId, IEnumerable<RequestPermissionTuple>? body, CancellationToken cancellationToken);
    Task AddTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId);
    Task AddTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task<List<Anonymous>> CreateObjectsAsync(IReadOnlyDictionary<string, string>? meta, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey);
    Task<List<Anonymous>> CreateObjectsAsync(IReadOnlyDictionary<string, string>? meta, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken);
    Task DeleteMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId);
    Task DeleteMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
    Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId);
    Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken);
    Task DeleteTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId);
    Task DeleteTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task<List<S3TagSet>> GetMetadataAsync(IReadOnlyDictionary<string, string>? meta);
    Task<List<S3TagSet>> GetMetadataAsync(IReadOnlyDictionary<string, string>? meta, CancellationToken cancellationToken);
    Task<List<S3TagSet>> GetTaggingAsync(IReadOnlyDictionary<string, string>? tags);
    Task<List<S3TagSet>> GetTaggingAsync(IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken);
    Task HeadObjectAsync(Guid objId, string? versionId);
    Task HeadObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken);
    Task<List<DBIdentityProvider>> ListIdpsAsync(bool? active);
    Task<List<DBIdentityProvider>> ListIdpsAsync(bool? active, CancellationToken cancellationToken);
    Task<S3VersionList> ListObjectVersionAsync(Guid objId);
    Task<S3VersionList> ListObjectVersionAsync(Guid objId, CancellationToken cancellationToken);
    Task<List<DBObjectPermission>> ListPermissionsAsync(Guid objId, Guid? userId, PermCode? permCode);
    Task<List<DBObjectPermission>> ListPermissionsAsync(Guid objId, Guid? userId, PermCode? permCode, CancellationToken cancellationToken);
    Task<FileResponse> ReadObjectAsync(Guid objId, DownloadMode? download, int? expiresIn, string? versionId);
    Task<FileResponse> ReadObjectAsync(Guid objId, DownloadMode? download, int? expiresIn, string? versionId, CancellationToken cancellationToken);
    Task<List<DBObjectPermission>> RemovePermissionsAsync(Guid objId, Guid? userId, PermCode? permCode);
    Task<List<DBObjectPermission>> RemovePermissionsAsync(Guid objId, Guid? userId, PermCode? permCode, CancellationToken cancellationToken);
    Task ReplaceMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId);
    Task ReplaceMetadataAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
    Task ReplaceTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId);
    Task ReplaceTaggingAsync(Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task<List<DBObject>> SearchObjectsAsync(IReadOnlyDictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? @public, string? mimeType, string? name, IReadOnlyDictionary<string, string>? tags);
    Task<List<DBObject>> SearchObjectsAsync(IReadOnlyDictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? @public, string? mimeType, string? name, IReadOnlyDictionary<string, string>? tags, CancellationToken cancellationToken);
    Task<List<DBObjectPermission>> SearchPermissionsAsync(Guid? objId, Guid? userId, PermCode? permCode);
    Task<List<DBObjectPermission>> SearchPermissionsAsync(Guid? objId, Guid? userId, PermCode? permCode, CancellationToken cancellationToken);
    Task<List<DBUser>> SearchUsersAsync(Guid? userId, Guid? identityId, string? idp, string? username, string? email, string? firstName, string? fullName, string? lastName, bool? active, string? search);
    Task<List<DBUser>> SearchUsersAsync(Guid? userId, Guid? identityId, string? idp, string? username, string? email, string? firstName, string? fullName, string? lastName, bool? active, string? search, CancellationToken cancellationToken);
    Task<DBObject> TogglePublicAsync(Guid objId, bool? @public);
    Task<DBObject> TogglePublicAsync(Guid objId, bool? @public, CancellationToken cancellationToken);
    Task<Response> UpdateObjectAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey);
    Task<Response> UpdateObjectAsync(IReadOnlyDictionary<string, string>? meta, Guid objId, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken);

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
}
