﻿namespace TrafficCourts.Coms.Client;

public class FileSearchParameters
{
    public FileSearchParameters()
        : this(null, null, null)
    {
    }

    public FileSearchParameters(Guid id)
        : this(new List<Guid> { id }, null, null)
    {
    }

    public FileSearchParameters(IEnumerable<Guid>? ids, IReadOnlyDictionary<string, string>? metadata, IReadOnlyDictionary<string, string>? tags)
    {
        Ids = ids is not null
            ? new List<Guid>(ids)
            : new List<Guid>();

        Metadata = Client.Metadata.Create(metadata);
        Tags = Factory.CreateTags(tags);
    }

    /// <summary>
    /// The list of ids to search for.
    /// </summary>
    public List<Guid> Ids { get; private set; }

    /// <summary>
    /// Metadata for the object
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; private set; }

    /// <summary>
    /// Tags for the object
    /// </summary>
    public IReadOnlyDictionary<string, string> Tags { get; private set; }

    /// <summary>
    /// The `name` source for the object Typically a descriptive title or original filename
    /// </summary>
    public string? Name { get; set; }

    public Guid? BucketId { get; set; }

    /// <summary>
    /// The canonical S3 path string of the object
    /// </summary>
    public string? Path { get; set; }
    
    /// <summary>
    /// The object MIME Type
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// Boolean on public status
    /// </summary>
    public bool? Public { get; set; }

    /// <summary>
    /// Boolean on active status
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Delete marker
    /// </summary>
    public bool? DeleteMarker { get; set; }

    /// <summary>
    /// Get the latest version. Defaults to <c>true</c>.
    /// </summary>
    public bool? Latest { get; set; } = true;
}
