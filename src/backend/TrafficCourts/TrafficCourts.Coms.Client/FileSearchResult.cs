namespace TrafficCourts.Coms.Client;

public class FileSearchResult : TimestampUserData
{
    public FileSearchResult(Guid id, string path, bool active, bool @public, Guid createdBy, DateTimeOffset createdAt, Guid updatedBy, DateTimeOffset updatedAt)
        : this(id, path, active, @public, createdBy, createdAt, updatedBy, updatedAt, null, null)
    {
    }

    public FileSearchResult(
        Guid id, 
        string path, 
        bool active, 
        bool @public, 
        Guid createdBy, 
        DateTimeOffset createdAt, 
        Guid updatedBy, 
        DateTimeOffset updatedAt, 
        IDictionary<string, string>? metadata,
        IDictionary<string, string>? tags)
    {
        Id = id;
        Path = path;
        Active = active;
        Public = @public;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
        Metadata = Factory.CreateMetadata(metadata);
        Tags = Factory.CreateTags(tags);
    }

    public Guid Id { get; set; }
    public string Path { get; set; }
    public bool Active { get; set; }
    public bool Public { get; set; }

    public Dictionary<string, string> Metadata { get; private set; }
    public Dictionary<string, string> Tags { get; private set; }
}
