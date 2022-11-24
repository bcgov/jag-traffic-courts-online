namespace TrafficCourts.Coms.Client;

public partial class File
{
    public File(Stream data)
        : this(data, null, null)
    {
    }

    public File(Stream data, string? fileName)
        : this(data, fileName, null)
    {
    }

    public File(Stream data, string? fileName, string? contentType)
        : this(data, fileName, contentType, null, null)
    {
    }

    public File(Stream data, string? fileName, string? contentType, IDictionary<string, string>? metadata, IDictionary<string, string>? tags)
    {
        Data = data;
        FileName = fileName;
        ContentType = contentType;

        Metadata = Factory.CreateMetadata(metadata);
        Tags = Factory.CreateTags(tags);
    }

    public Stream Data { get; private set; }

    public string? FileName { get; private set; }

    public string? ContentType { get; private set; }

    public Dictionary<string, string> Metadata { get; private set; }
    public Dictionary<string, string> Tags { get; private set; }
}
