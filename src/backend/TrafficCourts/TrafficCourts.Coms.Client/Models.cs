using System.Text.Json.Serialization;

namespace TrafficCourts.Coms.Client;

public class ObjectMetadataCollection : List<ObjectMetadata>
{
}

public class ObjectMetadata
{
    public ObjectMetadata()
    {
        Metadata = new List<MetadataItem>();
    }
    public string? VersionId { get; set; }

    [JsonPropertyName("objectId")]
    public Guid Id { get; set; }

    public string? MimeType { get; set; }

    public bool DeleteMarker { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("metadata")]
    public IList<MetadataItem> Metadata { get; set; }
}

public class MetadataItem
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
