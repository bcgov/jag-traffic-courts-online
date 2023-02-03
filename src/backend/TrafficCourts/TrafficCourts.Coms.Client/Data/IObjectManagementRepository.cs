namespace TrafficCourts.Coms.Client.Data;

public interface IObjectManagementRepository
{
    IList<KeyValuePair<string, string>> GetObjectMetadata(Guid objectId, string? versionId = null);
    IList<KeyValuePair<string, string>> GetObjectTags(Guid objectId, string? versionId = null);
}
