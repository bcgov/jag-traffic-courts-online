namespace TrafficCourts.Coms.Client;

public partial class File : IDisposable
{
    /// <summary>Is this already disposed?</summary>
    private bool _disposed;
    /// <summary>Does this instance own the memory stream and should dispose of it?</summary>
    private bool _ownsDataStream = true;

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

    public void Dispose()
    {
        Dispose(disposing: true);
        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SuppressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_ownsDataStream)
                {
                    Data.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
