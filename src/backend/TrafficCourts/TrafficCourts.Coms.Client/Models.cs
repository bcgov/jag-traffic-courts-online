using System.Text.Json.Serialization;

namespace TrafficCourts.Coms.Client;

public class ObjectMetadataCollection : List<ObjectMetadata>
{
}

public class ObjectMetadata
{
    public string? VersionId { get; set; }

    [JsonPropertyName("objectId")]
    public Guid Id { get; set; }

    public string? MimeType { get; set; }

    public bool DeleteMarker { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<MetadataItem> Metadata { get; set; }
}

public class MetadataItem
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
