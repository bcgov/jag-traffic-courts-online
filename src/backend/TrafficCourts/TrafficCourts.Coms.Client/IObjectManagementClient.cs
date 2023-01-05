namespace TrafficCourts.Coms.Client;

/// <summary>
/// Abstraction of generated <see cref="ObjectManagementClient"/>
/// </summary>
public interface IObjectManagementClient
{
    string BaseUrl { get; set; }
    bool ReadResponseAsString { get; set; }

    Task AddMetadataAsync(IDictionary<string, string>? meta, Guid objId, string? versionId);
    Task AddMetadataAsync(IDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
    Task<List<DBObjectPermission>> AddPermissionsAsync(Guid objId, IEnumerable<RequestPermissionTuple>? body);
    Task<List<DBObjectPermission>> AddPermissionsAsync(Guid objId, IEnumerable<RequestPermissionTuple>? body, CancellationToken cancellationToken);
    Task AddTaggingAsync(Guid objId, IDictionary<string, string>? tags, string? versionId);
    Task AddTaggingAsync(Guid objId, IDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task<List<Anonymous>> CreateObjectsAsync(IDictionary<string, string>? meta, IDictionary<string, string>? tags, FileParameter anyKey);
    Task<List<Anonymous>> CreateObjectsAsync(IDictionary<string, string>? meta, IDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken);
    Task DeleteMetadataAsync(IDictionary<string, string>? meta, Guid objId, string? versionId);
    Task DeleteMetadataAsync(IDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
    Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId);
    Task<ResponseObjectDeleted> DeleteObjectAsync(Guid objId, string? versionId, CancellationToken cancellationToken);
    Task DeleteTaggingAsync(Guid objId, IDictionary<string, string>? tags, string? versionId);
    Task DeleteTaggingAsync(Guid objId, IDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task<List<S3TagSet>> GetMetadataAsync(IDictionary<string, string>? meta);
    Task<List<S3TagSet>> GetMetadataAsync(IDictionary<string, string>? meta, CancellationToken cancellationToken);
    Task<List<S3TagSet>> GetTaggingAsync(IDictionary<string, string>? tags);
    Task<List<S3TagSet>> GetTaggingAsync(IDictionary<string, string>? tags, CancellationToken cancellationToken);
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
    Task ReplaceMetadataAsync(IDictionary<string, string>? meta, Guid objId, string? versionId);
    Task ReplaceMetadataAsync(IDictionary<string, string>? meta, Guid objId, string? versionId, CancellationToken cancellationToken);
    Task ReplaceTaggingAsync(Guid objId, IDictionary<string, string>? tags, string? versionId);
    Task ReplaceTaggingAsync(Guid objId, IDictionary<string, string>? tags, string? versionId, CancellationToken cancellationToken);
    Task<List<DBObject>> SearchObjectsAsync(IDictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? @public, string? mimeType, string? name, IDictionary<string, string>? tags);
    Task<List<DBObject>> SearchObjectsAsync(IDictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? @public, string? mimeType, string? name, IDictionary<string, string>? tags, CancellationToken cancellationToken);
    Task<List<DBObjectPermission>> SearchPermissionsAsync(Guid? objId, Guid? userId, PermCode? permCode);
    Task<List<DBObjectPermission>> SearchPermissionsAsync(Guid? objId, Guid? userId, PermCode? permCode, CancellationToken cancellationToken);
    Task<List<DBUser>> SearchUsersAsync(Guid? userId, Guid? identityId, string? idp, string? username, string? email, string? firstName, string? fullName, string? lastName, bool? active, string? search);
    Task<List<DBUser>> SearchUsersAsync(Guid? userId, Guid? identityId, string? idp, string? username, string? email, string? firstName, string? fullName, string? lastName, bool? active, string? search, CancellationToken cancellationToken);
    Task<DBObject> TogglePublicAsync(Guid objId, bool? @public);
    Task<DBObject> TogglePublicAsync(Guid objId, bool? @public, CancellationToken cancellationToken);
    Task<Response> UpdateObjectAsync(IDictionary<string, string>? meta, Guid objId, IDictionary<string, string>? tags, FileParameter anyKey);
    Task<Response> UpdateObjectAsync(IDictionary<string, string>? meta, Guid objId, IDictionary<string, string>? tags, FileParameter anyKey, CancellationToken cancellationToken);
}
