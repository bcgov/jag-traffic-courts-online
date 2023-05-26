namespace TrafficCourts.Coms.Client;

public class FileSearchResult : TimestampUserData
{
    public FileSearchResult(Guid id, string filename, string path, bool active, bool @public, Guid createdBy, DateTimeOffset createdAt, Guid updatedBy, DateTimeOffset updatedAt)
        : this(id, filename, path, active, @public, createdBy, createdAt, updatedBy, updatedAt, null, null)
    {
    }

    public FileSearchResult(
        Guid id, 
        string filename,
        string path, 
        bool active, 
        bool @public, 
        Guid createdBy, 
        DateTimeOffset createdAt, 
        Guid updatedBy, 
        DateTimeOffset? updatedAt,
        IReadOnlyDictionary<string, string>? metadata,
        IReadOnlyDictionary<string, string>? tags)
    {
        Id = id;
        FileName = filename;
        Path = path;
        Active = active;
        Public = @public;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
        Metadata = Client.Metadata.Create(metadata);
        Tags = Factory.CreateTags(tags);
    }

    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Path { get; set; }
    public bool Active { get; set; }
    public bool Public { get; set; }

    public IReadOnlyDictionary<string, string> Metadata { get; private set; }
    public IReadOnlyDictionary<string, string> Tags { get; private set; }
}
