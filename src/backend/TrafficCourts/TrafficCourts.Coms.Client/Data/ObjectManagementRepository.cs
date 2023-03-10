namespace TrafficCourts.Coms.Client.Data;

public class ObjectManagementRepository : IObjectManagementRepository
{
    private readonly ObjectManagementContext _context;

    public ObjectManagementRepository(ObjectManagementContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IList<KeyValuePair<string, string>> GetObjectTags(Guid objectId, string? versionId = null)
    {
        Guid id = versionId is not null 
            ? GetVersion(objectId, versionId)
            : GetLatestVersion(objectId);

        if (id == Guid.Empty)
        {
            Array.Empty<KeyValuePair<string, string>>();
        }

        var items = GetVersionTags(id);
        return items;
    }

    public IList<KeyValuePair<string, string>> GetObjectMetadata(Guid objectId, string? versionId = null)
    {
        Guid id = versionId is not null
            ? GetVersion(objectId, versionId)
            : GetLatestVersion(objectId);

        if (id == Guid.Empty)
        {
            Array.Empty<KeyValuePair<string, string>>();
        }

        var items = GetVersionMetadata(id);
        return items;
    }

    private IList<KeyValuePair<string, string>> GetVersionMetadata(Guid versionId)
    {
        var items = _context.VersionMetadata
            .Where(_ => _.VersionId == versionId)
            .Select(_ => new KeyValuePair<string, string>(_.Metadata.Key, _.Metadata.Value))
            .ToList();

        return items;
    }

    private IList<KeyValuePair<string, string>> GetVersionTags(Guid versionId)
    {
        var items = _context.VersionTags
            .Where(_ => _.VersionId == versionId)
            .Select(_ => new KeyValuePair<string, string>(_.Tag.Key, _.Tag.Value))
            .ToList();

        return items;
    }

    private Guid GetLatestVersion(Guid objectId)
    {
        Guid versionId = _context.Versions
            .Where(_ => _.ObjectId == objectId && _.DeleteMarker == false)
            .OrderByDescending(_ => _.CreatedAt)
            .Select(_ => _.Id)
            .FirstOrDefault();

        return versionId;
    }

    private Guid GetVersion(Guid objectId, string versionId)
    {
        Guid id = _context.Versions
            .Where(_ => _.ObjectId == objectId && _.S3VersionId == versionId)
            .Select(_ => _.Id)
            .SingleOrDefault();

        return id;
    }
}
