namespace TrafficCourts.Coms.Client;

public class MetadataInvalidKeyException : MetadataException
{
    public string? Key { get; private set; }

    public MetadataInvalidKeyException(string? key) : base("Metadata key is null, empty or contains invalid characters")
    {
        Key = key;
    }
}
